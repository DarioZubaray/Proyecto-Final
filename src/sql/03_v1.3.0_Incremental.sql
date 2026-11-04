USE [DarioZubaray_TF];
GO

-- =============================================
-- MIGRACION v1.3.0 (incremental)
--
-- Aplica sobre la base v1 (01_v1.0.0_CreateTables +
-- 02_v1.0.0_SeedData) todas las mejoras de esquema y
-- datos del sistema, consolidando en un solo script las
-- migraciones historicas:
--
--   1. v1.1.0 Aulas, cursos, inscripciones y asistencia
--      (tablas, permisos y seed de datos)
--   2. v1.2.0 Contrasena del usuario admin
--   3. v1.3.0 DVH: columna users.dvh + backfill SHA-256
--   4. v1.4.0 Tickets + permiso FORM_TICKET_MGMT
--   5. v1.4.1 Alumnos solo datos + tickets al Coordinador
--   6. v1.5.0 Entidad students, rol Director y
--      permiso FORM_STUDENT_MGMT
--   7. v1.6.0 Evaluaciones y notas (FORM_EVAL_MGMT)
--   8. v1.7.0 Informes personalizados
--      (FORM_REPORT_DETAIL)
--   9. v1.8.0 Respaldo y restauracion
--      (FORM_BACKUP_MGMT)
--
-- Todas las secciones son idempotentes: el script puede
-- ejecutarse sobre una base v1, sobre una base con
-- migraciones legadas o repetirse sin efecto. Registra la
-- version '1.3.0' en [schema_versions].
--
-- Uso: 01 -> 02 -> 03   (o el consolidado 05 en un paso)
-- Rollback: 04_v1.3.0_Rollback.sql
-- =============================================

-- =============================================
-- Lote previo: columna DVH.
-- SQL Server compila cada lote completo contra el
-- esquema previo a su ejecucion: un UPDATE que
-- referencia una columna recien agregada con ALTER
-- TABLE en el mismo lote fallaria con "Invalid
-- column name". Por eso el ALTER va en un lote
-- propio, antes de la migracion.
-- =============================================
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.users', 'dvh') IS NULL
    ALTER TABLE [dbo].[users] ADD [dvh] NVARCHAR(64) NULL;
GO

-- Se omite si la version 1.3.0 ya fue registrada por este
-- script o por el consolidado (05). Una fila legada '1.3.0'
-- de la antigua migracion DVH (script 08_v1.3.0_DVH.sql) no
-- bloquea la ejecucion: las secciones son idempotentes.
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[schema_versions]
    WHERE [version] = '1.3.0'
      AND [script_name] IN ('03_v1.3.0_Incremental.sql', '05_v1.3.0_ScriptConsolidado.sql')
)
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. v1.1.0: aulas, cursos, inscripciones,
        --    asistencia (tablas + permisos + seed)
        -- =============================================

        -- 1.1 Aulas
        IF OBJECT_ID('dbo.classrooms', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[classrooms] (
                [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [name]         NVARCHAR(100)  NOT NULL,
                [capacity]     INT            NOT NULL,
                [is_active]    BIT            NOT NULL DEFAULT 1,
                [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
            );
        END;

        -- 1.2 Cursos (con horario; se adapta a instalaciones viejas)
        IF OBJECT_ID('dbo.courses', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[courses] (
                [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [name]          NVARCHAR(200)  NOT NULL,
                [description]   NVARCHAR(500)  NULL,
                [start_date]    DATETIME       NOT NULL,
                [end_date]      DATETIME       NOT NULL,
                [classroom_id]  INT            NOT NULL,
                [day_of_week]   INT            NULL,
                [start_time]    TIME           NULL,
                [end_time]      TIME           NULL,
                [is_active]     BIT            NOT NULL DEFAULT 1,
                [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_courses_classrooms FOREIGN KEY ([classroom_id]) REFERENCES [dbo].[classrooms]([id])
            );
        END
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.courses') AND name = 'classroom_id' AND is_nullable = 1)
            BEGIN
                UPDATE [dbo].[courses] SET [classroom_id] = 1 WHERE [classroom_id] IS NULL;
                ALTER TABLE [dbo].[courses] ALTER COLUMN [classroom_id] INT NOT NULL;
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.courses') AND name = 'day_of_week')
            BEGIN
                ALTER TABLE [dbo].[courses]
                    ADD [day_of_week]  INT  NULL,
                        [start_time]   TIME NULL,
                        [end_time]     TIME NULL;
            END;
        END;

        -- 1.3 Docentes por curso (N:N)
        IF OBJECT_ID('dbo.course_teachers', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[course_teachers] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [teacher_id]   INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_course_teachers_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_course_teachers_users   FOREIGN KEY ([teacher_id]) REFERENCES [dbo].[users]([id])
            );
        END;

        -- 1.4 Inscripciones
        IF OBJECT_ID('dbo.course_students', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[course_students] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [student_id]   INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_course_students_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_course_students_users   FOREIGN KEY ([student_id]) REFERENCES [dbo].[users]([id])
            );
        END;

        -- 1.5 Asistencia
        IF OBJECT_ID('dbo.attendance', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[attendance] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [student_id]   INT      NOT NULL,
                [date]         DATE     NOT NULL,
                [is_present]   BIT      NOT NULL DEFAULT 0,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_attendance_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_attendance_users   FOREIGN KEY ([student_id]) REFERENCES [dbo].[users]([id]),
                CONSTRAINT uq_attendance_unique  UNIQUE ([course_id], [student_id], [date])
            );
        END;

        -- 1.6 Permisos academicos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_CURSO_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_CURSO_MGMT', 'ABM Cursos', 'Formulario de gestion de cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_INSCRIPCION_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_INSCRIPCION_MGMT', 'Inscripciones', 'Gestion de inscripciones de alumnos a cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_ASISTENCIA_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_MGMT', 'Asistencia', 'Registro de asistencia de alumnos en clases', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_ASISTENCIA_VIEW')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_VIEW', 'Ver Asistencia', 'Consulta de asistencia de alumnos', 0);
        END;

        -- 1.7 Rol Coordinador (por si no existe)
        IF NOT EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Coordinador')
        BEGIN
            INSERT INTO [dbo].[roles] (name) VALUES ('Coordinador');
        END;

        -- 1.8 Asignacion de permisos por rol

        -- Admin (1): permisos academicos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        -- Coordinador (4): permisos del diario (aulas/cursos,
        -- inscripciones y consulta de asistencia). Se otorgan uno
        -- por uno: la version historica solo los asignaba si el rol
        -- no tenia ninguna asignacion, por lo que nunca se aplicaba
        -- sobre el seed.
        DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[roles] WHERE name = 'Coordinador');
        IF @coordinadorId IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT'))
            BEGIN
                INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
                SELECT @coordinadorId, id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT';
            END;

            IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
            BEGIN
                INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
                SELECT @coordinadorId, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
            END;

            IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_VIEW'))
            BEGIN
                INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
                SELECT @coordinadorId, id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_VIEW';
            END;
        END;

        -- Profesor (2): opera sus cursos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        DELETE FROM [dbo].[role_permissions]
        WHERE [role_id] = 2
          AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_USER_MGMT');

        -- Alumno (3): se inscribe a sus cursos (solo si el rol existe;
        -- en bases migradas a v1.5.0+ el rol Alumno ya fue eliminado)
        IF EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno')
        AND NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 3 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 3, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- 1.9 Seed: aulas
        IF NOT EXISTS (SELECT 1 FROM [dbo].[classrooms])
        BEGIN
            INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
            ('Aula 101 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 102 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 201 - Goiescalza',      25, 1, GETDATE(), GETDATE()),
            ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
            ('Aula Magna - Goiescalza',   100, 1, GETDATE(), GETDATE());
        END;

        -- 1.10 Seed: cursos con horario
        IF NOT EXISTS (SELECT 1 FROM [dbo].[courses])
        BEGIN
            INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
            ('Ingles Basico A1',     'Curso de ingles para principiantes, nivel A1 del MCER',             '2026-08-01', '2026-12-15', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
            ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion',  '2026-08-01', '2026-12-15', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
            ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',         '2026-08-01', '2026-12-15', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
            ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',           '2026-08-01', '2026-12-15', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
            ('Italiano A1',          'Curso de italiano para principantes, conversacion y gramatica',     '2026-08-01', '2026-12-15', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());
        END;

        -- 1.11 Seed: docentes por curso
        IF NOT EXISTS (SELECT 1 FROM [dbo].[course_teachers])
        BEGIN
            INSERT INTO [dbo].[course_teachers] (course_id, teacher_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                (1, 'prof_garcia'),
                (1, 'prof_lopez'),
                (2, 'prof_martinez'),
                (3, 'prof_rodriguez'),
                (4, 'prof_fernandez'),
                (4, 'prof_garcia'),
                (5, 'prof_lopez')
            ) AS t(course_num, user_name)
            INNER JOIN [dbo].[courses] c ON c.id = t.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = t.user_name;
        END;

        -- 1.12 Seed: inscripciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[course_students])
        BEGIN
            INSERT INTO [dbo].[course_students] (course_id, student_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                (1, 'alumno_perez'),
                (1, 'alumno_gomez'),
                (1, 'alumno_diaz'),
                (1, 'alumno_morales'),
                (1, 'alumno_torres'),
                (2, 'alumno_ramos'),
                (2, 'alumno_silva'),
                (2, 'alumno_castro'),
                (3, 'alumno_ruiz'),
                (3, 'alumno_alvarez'),
                (3, 'alumno_romero'),
                (3, 'alumno_sanchez'),
                (4, 'alumno_herrera'),
                (4, 'alumno_patricio'),
                (4, 'alumno_soto'),
                (5, 'alumno_perez'),
                (5, 'alumno_ruiz')
            ) AS e(course_num, user_name)
            INNER JOIN [dbo].[courses] c ON c.id = e.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = e.user_name;
        END;

        -- 1.13 Seed: asistencia
        IF NOT EXISTS (SELECT 1 FROM [dbo].[attendance])
        BEGIN
            INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
            SELECT c.id, u.id, a.date, a.is_present, GETDATE()
            FROM (VALUES
                (1, 'alumno_perez',   '2026-08-03', 1),
                (1, 'alumno_gomez',   '2026-08-03', 1),
                (1, 'alumno_diaz',    '2026-08-03', 0),
                (1, 'alumno_morales', '2026-08-03', 1),
                (1, 'alumno_torres',  '2026-08-03', 1),
                (1, 'alumno_perez',   '2026-08-10', 1),
                (1, 'alumno_gomez',   '2026-08-10', 0),
                (1, 'alumno_diaz',    '2026-08-10', 1),
                (1, 'alumno_morales', '2026-08-10', 1),
                (1, 'alumno_torres',  '2026-08-10', 1),
                (2, 'alumno_ramos',   '2026-08-05', 1),
                (2, 'alumno_silva',   '2026-08-05', 1),
                (2, 'alumno_castro',  '2026-08-05', 0),
                (2, 'alumno_ramos',   '2026-08-12', 1),
                (2, 'alumno_silva',   '2026-08-12', 0),
                (2, 'alumno_castro',  '2026-08-12', 1),
                (3, 'alumno_ruiz',      '2026-08-04', 1),
                (3, 'alumno_alvarez',   '2026-08-04', 1),
                (3, 'alumno_romero',    '2026-08-04', 1),
                (3, 'alumno_sanchez',   '2026-08-04', 0),
                (4, 'alumno_herrera',   '2026-08-06', 1),
                (4, 'alumno_patricio',  '2026-08-06', 0),
                (4, 'alumno_soto',      '2026-08-06', 1),
                (5, 'alumno_perez', '2026-08-07', 1),
                (5, 'alumno_ruiz',  '2026-08-07', 1)
            ) AS a(course_num, user_name, date, is_present)
            INNER JOIN [dbo].[courses] c ON c.id = a.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = a.user_name;
        END;

        -- =============================================
        -- 2. v1.2.0: contrasena del usuario admin
        -- =============================================

        -- Hash BCrypt para password "admin": $2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G
        UPDATE [dbo].[users]
        SET [password_hash] = '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G',
            [is_active]     = 1,
            [retries_count] = 0,
            [last_update]   = GETDATE()
        WHERE [user_name] = 'admin';

        -- =============================================
        -- 3. v1.3.0: backfill del DVH
        --
        -- La columna [dvh] se crea en el lote previo
        -- a la migracion. Replica la serializacion
        -- canonica de la app (BLL.DVHHelperBLL):
        -- columnas menos [dvh], en orden de columna,
        -- unidas por coma; fechas estilo 126;
        -- role_id NULL -> ''; SHA2_256 sobre NVARCHAR
        -- (bytes UTF-16LE, igual que Encoding.Unicode).
        -- =============================================
        UPDATE u
        SET u.[dvh] = LOWER(CONVERT(VARCHAR(64),
            HASHBYTES('SHA2_256',
                CONCAT(
                    CONVERT(NVARCHAR(MAX), u.[id]),          ',',
                    u.[user_name],                           ',',
                    u.[password_hash],                       ',',
                    CASE WHEN u.[is_active] = 1 THEN '1' ELSE '0' END, ',',
                    CONVERT(NVARCHAR(MAX), u.[retries_count]), ',',
                    CONVERT(NVARCHAR(30), u.[last_update], 126), ',',
                    CONVERT(NVARCHAR(30), u.[created_at], 126),  ',',
                    CASE WHEN u.[role_id] IS NULL THEN '' ELSE CONVERT(NVARCHAR(MAX), u.[role_id]) END, ',',
                    u.[language],                            ',',
                    u.[theme]
                )
            ), 2))
        FROM [dbo].[users] u
        WHERE u.[dvh] IS NULL;

        -- =============================================
        -- 4. v1.4.0: tickets + permiso de gestion
        -- =============================================
        IF OBJECT_ID('dbo.tickets', 'U') IS NOT NULL
        BEGIN
            -- Tabla creada por una instalacion previa distinta: se recrea limpia.
            DROP TABLE [dbo].[tickets];
        END;

        CREATE TABLE [dbo].[tickets] (
            [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
            [user_id]     INT            NOT NULL,
            [subject]     NVARCHAR(200)  NOT NULL,
            [description] NVARCHAR(MAX)  NOT NULL,
            [is_closed]   BIT            NOT NULL DEFAULT 0,
            [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
            [closed_at]   DATETIME       NULL,
            [closed_by]   INT            NULL,
            CONSTRAINT fk_tickets_user FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]),
            CONSTRAINT fk_tickets_closed_by FOREIGN KEY ([closed_by]) REFERENCES [dbo].[users]([id])
        );
        CREATE INDEX ix_tickets_user_created ON [dbo].[tickets] ([user_id], [created_at] DESC);
        CREATE INDEX ix_tickets_is_closed ON [dbo].[tickets] ([is_closed], [created_at] DESC);

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_TICKET_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system)
            VALUES ('FORM_TICKET_MGMT', 'Gestion de Tickets', 'Listar y cerrar tickets de todos los usuarios', 0);
        END;

        -- Asignacion al rol Admin (1); los demas roles crean tickets sin necesitar permiso.
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions]
            WHERE [role_id] = 1
              AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT')
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT';
        END;

        -- =============================================
        -- 5. v1.4.1: ajuste de actores
        -- =============================================

        -- Alumnos: solo datos (sin login)
        UPDATE u
        SET u.[is_active] = 0
        FROM [dbo].[users] u
        INNER JOIN [dbo].[roles] r ON r.[id] = u.[role_id]
        WHERE r.[name] = 'Alumno'
          AND u.[is_active] = 1;

        -- Tickets: el Coordinador gestiona (listar todos y cerrar)
        IF EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_TICKET_MGMT')
        AND EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Coordinador')
        AND NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions]
            WHERE [role_id] = (SELECT id FROM [dbo].[roles] WHERE name = 'Coordinador')
              AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT')
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT r.[id], p.[id]
            FROM [dbo].[roles] r
            CROSS JOIN [dbo].[permissions] p
            WHERE r.[name] = 'Coordinador'
              AND p.[name] = 'FORM_TICKET_MGMT';
        END;

        -- =============================================
        -- 6. v1.5.0: entidad students + roles finales
        -- =============================================

        -- 6.1 Tabla de alumnos (entidad gestionada)
        IF OBJECT_ID('[dbo].[students]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[students] (
                [id]            INT            IDENTITY(1,1) NOT NULL,
                [first_name]    NVARCHAR(100)  NOT NULL,
                [last_name]     NVARCHAR(100)  NOT NULL,
                [document]      NVARCHAR(20)   NOT NULL,
                [email]         NVARCHAR(150)  NULL,
                [phone]         NVARCHAR(50)   NULL,
                [is_active]     BIT            NOT NULL,
                [created_at]    DATETIME       NOT NULL,
                [last_update]   DATETIME       NOT NULL,
                CONSTRAINT [pk_students] PRIMARY KEY ([id]),
                CONSTRAINT [uq_students_document] UNIQUE ([document])
            );

            CREATE INDEX ix_students_name ON [dbo].[students] ([last_name], [first_name]);
        END;

        -- 6.2 Migracion de usuarios-alumno a students
        --     (mismos IDs: course_students/attendance no cambian)
        IF EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno')
        AND EXISTS (SELECT 1 FROM [dbo].[users] WHERE [role_id] = 3)
        BEGIN
            SET IDENTITY_INSERT [dbo].[students] ON;

            INSERT INTO [dbo].[students] ([id], [first_name], [last_name], [document], [email], [phone], [is_active], [created_at], [last_update])
            SELECT u.[id],
                   CASE WHEN CHARINDEX('_', u.[user_name]) > 0
                        THEN LEFT(u.[user_name], CHARINDEX('_', u.[user_name]) - 1)
                        ELSE u.[user_name] END,
                   CASE WHEN CHARINDEX('_', u.[user_name]) > 0
                        THEN SUBSTRING(u.[user_name], CHARINDEX('_', u.[user_name]) + 1, LEN(u.[user_name]))
                        ELSE u.[user_name] END,
                   'DNI-' + RIGHT('00000000' + CAST(u.[id] AS NVARCHAR(10)), 8),
                   NULL,
                   NULL,
                   1,
                   u.[created_at],
                   GETDATE()
            FROM [dbo].[users] u
            WHERE u.[role_id] = 3
              AND NOT EXISTS (SELECT 1 FROM [dbo].[students] s WHERE s.[id] = u.[id]);

            SET IDENTITY_INSERT [dbo].[students] OFF;

            -- Repuntar FKs de course_students/attendance
            IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = 'fk_course_students_users')
                ALTER TABLE [dbo].[course_students] DROP CONSTRAINT [fk_course_students_users];

            IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = 'fk_attendance_users')
                ALTER TABLE [dbo].[attendance] DROP CONSTRAINT [fk_attendance_users];

            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = 'fk_course_students_students')
                ALTER TABLE [dbo].[course_students] WITH CHECK
                    ADD CONSTRAINT [fk_course_students_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]);

            IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE [name] = 'fk_attendance_students')
                ALTER TABLE [dbo].[attendance] WITH CHECK
                    ADD CONSTRAINT [fk_attendance_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]);

            -- Eliminar usuarios-alumno y rol Alumno
            -- (dependencias FK primero: tickets y bitacora)
            DELETE t
            FROM [dbo].[tickets] t
            INNER JOIN [dbo].[users] u ON u.[id] = t.[user_id]
            WHERE u.[role_id] = 3;

            DELETE a
            FROM [dbo].[activity_logs] a
            INNER JOIN [dbo].[users] u ON u.[id] = a.[user_id]
            WHERE u.[role_id] = 3;

            DELETE FROM [dbo].[users] WHERE [role_id] = 3;

            DELETE FROM [dbo].[role_permissions] WHERE [role_id] = 3;
            DELETE FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno';
        END;

        -- 6.3 Rol Director (solo consulta de reportes). Se crea con el
        --     id 5 fijo: los ajustes de permisos de este script (y los
        --     de las migraciones historicas) referencian el rol 5.
        IF NOT EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Director')
        BEGIN
            SET IDENTITY_INSERT [dbo].[roles] ON;

            INSERT INTO [dbo].[roles] ([id], [name]) VALUES (5, 'Director');

            SET IDENTITY_INSERT [dbo].[roles] OFF;
        END;

        -- 6.4 Permiso ABM de alumnos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_STUDENT_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_STUDENT_MGMT', 'ABM Alumnos', 'Gestion de alumnos (entidad, no usuarios)', 0);
        END;

        -- 6.5 Ajuste de permisos por rol

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2): operador especial; sin ABM de cursos/aulas ni inscripciones
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 2
          AND p.[name] IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT');

        -- Coordinador (4): tareas diarias; sin usuarios/roles (son de Admin)
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 4
          AND p.[name] IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT');

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 4 AND p.[name] = 'FORM_STUDENT_MGMT'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 4, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_STUDENT_MGMT';
        END;

        -- Director (5): solo consulta de reportes
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        -- =============================================
        -- 7. v1.6.0: evaluaciones y notas
        -- =============================================

        -- 7.1 Tabla de evaluaciones
        IF OBJECT_ID('[dbo].[evaluations]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[evaluations] (
                [id]           INT            IDENTITY(1,1) NOT NULL,
                [course_id]    INT            NOT NULL,
                [name]         NVARCHAR(200)  NOT NULL,
                [date]         DATE           NOT NULL,
                [created_by]   INT            NOT NULL,
                [created_at]   DATETIME       NOT NULL,
                [last_update]  DATETIME       NOT NULL,
                CONSTRAINT [pk_evaluations] PRIMARY KEY ([id]),
                CONSTRAINT [fk_evaluations_courses] FOREIGN KEY ([course_id]) REFERENCES [dbo].[courses]([id]),
                CONSTRAINT [fk_evaluations_users] FOREIGN KEY ([created_by]) REFERENCES [dbo].[users]([id])
            );

            CREATE INDEX ix_evaluations_course ON [dbo].[evaluations] ([course_id], [date]);
        END;

        -- 7.2 Tabla de notas
        IF OBJECT_ID('[dbo].[grades]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[grades] (
                [id]             INT           IDENTITY(1,1) NOT NULL,
                [evaluation_id]  INT           NOT NULL,
                [student_id]     INT           NOT NULL,
                [grade]          DECIMAL(4,2)  NOT NULL,
                CONSTRAINT [pk_grades] PRIMARY KEY ([id]),
                CONSTRAINT [fk_grades_evaluations] FOREIGN KEY ([evaluation_id]) REFERENCES [dbo].[evaluations]([id]),
                CONSTRAINT [fk_grades_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]),
                CONSTRAINT [uq_grades_evaluation_student] UNIQUE ([evaluation_id], [student_id]),
                CONSTRAINT [ck_grades_range] CHECK ([grade] >= 0 AND [grade] <= 10)
            );
        END;

        -- 7.3 Permiso de evaluaciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_EVAL_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_EVAL_MGMT', 'Evaluaciones', 'Registro de evaluaciones y notas de los cursos del docente', 0);
        END;

        -- 7.4 Ajuste de permisos por rol

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2): otorga FORM_EVAL_MGMT si falta
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 2 AND p.[name] = 'FORM_EVAL_MGMT'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 2, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_EVAL_MGMT';
        END;

        -- Coordinador (4) y Director (5): sin FORM_EVAL_MGMT
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] IN (4, 5)
          AND p.[name] = 'FORM_EVAL_MGMT';

        -- =============================================
        -- 8. v1.7.0: informes personalizados
        -- =============================================

        -- 8.1 Permiso de informes personalizados
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_REPORT_DETAIL')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_REPORT_DETAIL', 'Informes Personalizados', 'Generacion y exportacion de informes academicos por curso', 0);
        END;

        -- 8.2 Ajuste de permisos por rol

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Coordinador (4): sin reportes (CU-13 y CU-14 son de Admin y Director)
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 4
          AND p.[name] IN ('FORM_REPORTS', 'FORM_REPORT_DETAIL');

        -- Director (5): consulta de reportes (CU-13 y CU-14)
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORT_DETAIL'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORT_DETAIL';
        END;

        -- Profesor (2): sin reportes (CU-13 y CU-14 son de Admin y Director)
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 2
          AND p.[name] IN ('FORM_REPORTS', 'FORM_REPORT_DETAIL');

        -- =============================================
        -- 9. v1.8.0: respaldo y restauracion
        -- =============================================

        -- 9.1 Permiso de respaldo
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_BACKUP_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_BACKUP_MGMT', 'Respaldo y Restauracion', 'Generacion de respaldos y restauracion de la base de datos', 0);
        END;

        -- 9.2 Ajuste de permisos por rol

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2), Coordinador (4) y Director (5): sin FORM_BACKUP_MGMT
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] IN (2, 4, 5)
          AND p.[name] = 'FORM_BACKUP_MGMT';

        -- =============================================
        -- 10. Registro de la migracion
        -- =============================================
        INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
        VALUES ('1.3.0', '03_v1.3.0_Incremental.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.3.0 (incremental) aplicada exitosamente.';

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        PRINT 'ERROR en la migracion v1.3.0: ' + @ErrorMessage;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
ELSE
BEGIN
    PRINT 'La migracion v1.3.0 (incremental) ya fue aplicada.';
END
GO
