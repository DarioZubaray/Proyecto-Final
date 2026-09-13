use Trabajo_Final;

-- =============================================
-- MIGRACION v1.1.0: Aulas, Cursos, Inscripciones y Asistencia
-- Create tables + Seed data (inscripciones + asistencia)
-- =============================================

-- 1. Verificar si esta migracion ya fue aplicada
IF NOT EXISTS (SELECT 1 FROM [dbo].[SchemaVersions] WHERE [Version] = '1.1.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 2. Tabla Aulas (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Aulas', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Aulas] (
                [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [nombre]       NVARCHAR(100)  NOT NULL,
                [capacidad]    INT            NOT NULL,
                [is_active]    BIT            NOT NULL DEFAULT 1,
                [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
            );
        END;

        -- =============================================
        -- 3. Tabla Cursos (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Cursos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Cursos] (
                [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [nombre]        NVARCHAR(200)  NOT NULL,
                [descripcion]   NVARCHAR(500)  NULL,
                [fecha_inicio]  DATETIME       NOT NULL,
                [fecha_fin]     DATETIME       NOT NULL,
                [aula_id]       INT            NOT NULL,
                [dia_semana]    INT            NULL,
                [hora_inicio]   TIME           NULL,
                [hora_fin]      TIME           NULL,
                [is_active]     BIT            NOT NULL DEFAULT 1,
                [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursos_aulas FOREIGN KEY ([aula_id]) REFERENCES [dbo].[Aulas]([id])
            );
        END;
        ELSE
        BEGIN
            -- Si la tabla existe pero aula_id es NULL, actualizar datos y cambiar a NOT NULL
            IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cursos') AND name = 'aula_id' AND is_nullable = 1)
            BEGIN
                UPDATE [dbo].[Cursos]
                SET [aula_id] = 1
                WHERE [aula_id] IS NULL;

                ALTER TABLE [dbo].[Cursos]
                    ALTER COLUMN [aula_id] INT NOT NULL;
            END;

            -- Agregar columnas de horario si no existen
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Cursos') AND name = 'dia_semana')
            BEGIN
                ALTER TABLE [dbo].[Cursos]
                    ADD [dia_semana]  INT  NULL,
                        [hora_inicio] TIME NULL,
                        [hora_fin]    TIME NULL;
            END;
        END;

        -- =============================================
        -- 4. Tabla CursoDocentes (N:N, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CursoDocentes] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [curso_id]     INT      NOT NULL,
                [docente_id]   INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursodocentes_cursos   FOREIGN KEY ([curso_id])   REFERENCES [dbo].[Cursos]([id]),
                CONSTRAINT fk_cursodocentes_users    FOREIGN KEY ([docente_id]) REFERENCES [dbo].[Users]([id])
            );
        END;

        -- =============================================
        -- 5. Tabla CursoAlumnos (inscripciones, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.CursoAlumnos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CursoAlumnos] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [curso_id]     INT      NOT NULL,
                [alumno_id]    INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursoalumnos_cursos FOREIGN KEY ([curso_id])  REFERENCES [dbo].[Cursos]([id]),
                CONSTRAINT fk_cursoalumnos_users  FOREIGN KEY ([alumno_id]) REFERENCES [dbo].[Users]([id])
            );
        END;

        -- =============================================
        -- 6. Tabla ClasesAlumnos (asistencia, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.ClasesAlumnos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[ClasesAlumnos] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [curso_id]     INT      NOT NULL,
                [alumno_id]    INT      NOT NULL,
                [fecha]        DATE     NOT NULL,
                [presente]     BIT      NOT NULL DEFAULT 0,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_clasesalumnos_cursos FOREIGN KEY ([curso_id])  REFERENCES [dbo].[Cursos]([id]),
                CONSTRAINT fk_clasesalumnos_users  FOREIGN KEY ([alumno_id]) REFERENCES [dbo].[Users]([id]),
                CONSTRAINT uq_clasesalumnos_unique  UNIQUE ([curso_id], [alumno_id], [fecha])
            );
        END;

        -- =============================================
        -- 7. Permisos
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_CURSO_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_CURSO_MGMT', 'ABM Cursos', 'Formulario de gestion de cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_INSCRIPCION_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_INSCRIPCION_MGMT', 'Inscripciones', 'Gestion de inscripciones de alumnos a cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_ASISTENCIA_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_MGMT', 'Asistencia', 'Registro de asistencia de alumnos en clases', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_ASISTENCIA_VIEW')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_VIEW', 'Ver Asistencia', 'Consulta de asistencia de alumnos', 0);
        END;

        -- =============================================
        -- 8. Rol Coordinador
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [name] = 'Coordinador')
        BEGIN
            INSERT INTO [dbo].[Roles] (name) VALUES ('Coordinador');
        END;

        -- =============================================
        -- 9. Asignacion de permisos
        -- =============================================

        -- Admin (1): todos los permisos nuevos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        -- Coordinador (4): ABMs + ver asistencia
        DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId)
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @coordinadorId, id FROM [dbo].[Permissions]
            WHERE name IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_ASISTENCIA_VIEW');
        END;

        -- Profesor (2): cursos, inscripciones, registrar asistencia
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        -- Quitar FORM_USER_MGMT a Profesor (no debe ver administracion)
        DELETE FROM [dbo].[RolePermissions]
        WHERE [role_id] = 2
          AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_USER_MGMT');

        -- Alumno (3): inscripciones
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 3 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 3, id FROM [dbo].[Permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- =============================================
        -- 10. Seed data: Aulas
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Aulas])
        BEGIN
            INSERT INTO [dbo].[Aulas] (nombre, capacidad, is_active, created_at, last_update) VALUES
            ('Aula 101 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 102 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 201 - Goiescalza',      25, 1, GETDATE(), GETDATE()),
            ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
            ('Aula Magna - Goiescalza',   100, 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 11. Seed data: Cursos con horarios
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Cursos])
        BEGIN
            INSERT INTO [dbo].[Cursos] (nombre, descripcion, fecha_inicio, fecha_fin, aula_id, dia_semana, hora_inicio, hora_fin, is_active, created_at, last_update) VALUES
            ('Ingles Basico A1',     'Curso de ingles para principiantes, nivel A1 del MCER',             '2026-08-01', '2026-12-15', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
            ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion',  '2026-08-01', '2026-12-15', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
            ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',         '2026-08-01', '2026-12-15', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
            ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',           '2026-08-01', '2026-12-15', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
            ('Italiano A1',          'Curso de italiano para principantes, conversacion y gramatica',     '2026-08-01', '2026-12-15', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 12. Seed data: Asignacion de docentes a cursos
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[CursoDocentes])
        BEGIN
            INSERT INTO [dbo].[CursoDocentes] (curso_id, docente_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                (1, 'prof_garcia'),
                (1, 'prof_lopez'),
                (2, 'prof_martinez'),
                (3, 'prof_rodriguez'),
                (4, 'prof_fernandez'),
                (4, 'prof_garcia'),
                (5, 'prof_lopez')
            ) AS assignments(curso_num, user_name)
            INNER JOIN [dbo].[Cursos] c ON c.id = assignments.curso_num
            INNER JOIN [dbo].[Users] u ON u.user_name = assignments.user_name;
        END;

        -- =============================================
        -- 13. Seed data: Inscripciones de alumnos a cursos
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[CursoAlumnos])
        BEGIN
            INSERT INTO [dbo].[CursoAlumnos] (curso_id, alumno_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                -- Ingles Basico A1 (curso 1): 5 alumnos
                (1, 'alumno_perez'),
                (1, 'alumno_gomez'),
                (1, 'alumno_diaz'),
                (1, 'alumno_morales'),
                (1, 'alumno_torres'),
                -- Ingles Intermedio B1 (curso 2): 3 alumnos
                (2, 'alumno_ramos'),
                (2, 'alumno_silva'),
                (2, 'alumno_castro'),
                -- Frances Inicial A1 (curso 3): 4 alumnos
                (3, 'alumno_ruiz'),
                (3, 'alumno_alvarez'),
                (3, 'alumno_romero'),
                (3, 'alumno_sanchez'),
                -- Portugues Basico (curso 4): 3 alumnos
                (4, 'alumno_herrera'),
                (4, 'alumno_patricio'),
                (4, 'alumno_soto'),
                -- Italiano A1 (curso 5): 2 alumnos
                (5, 'alumno_perez'),
                (5, 'alumno_ruiz')
            ) AS enrollments(curso_num, user_name)
            INNER JOIN [dbo].[Cursos] c ON c.id = enrollments.curso_num
            INNER JOIN [dbo].[Users] u ON u.user_name = enrollments.user_name;
        END;

        -- =============================================
        -- 14. Seed data: Registros de asistencia
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[ClasesAlumnos])
        BEGIN
            INSERT INTO [dbo].[ClasesAlumnos] (curso_id, alumno_id, fecha, presente, created_at)
            SELECT c.id, u.id, att.fecha, att.presente, GETDATE()
            FROM (VALUES
                -- Ingles Basico A1 - Lunes 03/08/2026
                (1, 'alumno_perez',   '2026-08-03', 1),
                (1, 'alumno_gomez',   '2026-08-03', 1),
                (1, 'alumno_diaz',    '2026-08-03', 0),
                (1, 'alumno_morales', '2026-08-03', 1),
                (1, 'alumno_torres',  '2026-08-03', 1),
                -- Ingles Basico A1 - Lunes 10/08/2026
                (1, 'alumno_perez',   '2026-08-10', 1),
                (1, 'alumno_gomez',   '2026-08-10', 0),
                (1, 'alumno_diaz',    '2026-08-10', 1),
                (1, 'alumno_morales', '2026-08-10', 1),
                (1, 'alumno_torres',  '2026-08-10', 1),
                -- Ingles Intermedio B1 - Miercoles 05/08/2026
                (2, 'alumno_ramos',  '2026-08-05', 1),
                (2, 'alumno_silva',  '2026-08-05', 1),
                (2, 'alumno_castro', '2026-08-05', 0),
                -- Ingles Intermedio B1 - Miercoles 12/08/2026
                (2, 'alumno_ramos',  '2026-08-12', 1),
                (2, 'alumno_silva',  '2026-08-12', 0),
                (2, 'alumno_castro', '2026-08-12', 1),
                -- Frances Inicial A1 - Martes 04/08/2026
                (3, 'alumno_ruiz',     '2026-08-04', 1),
                (3, 'alumno_alvarez',  '2026-08-04', 1),
                (3, 'alumno_romero',   '2026-08-04', 1),
                (3, 'alumno_sanchez',  '2026-08-04', 0),
                -- Portugues Basico - Jueves 06/08/2026
                (4, 'alumno_herrera',  '2026-08-06', 1),
                (4, 'alumno_patricio', '2026-08-06', 0),
                (4, 'alumno_soto',     '2026-08-06', 1),
                -- Italiano A1 - Viernes 07/08/2026
                (5, 'alumno_perez', '2026-08-07', 1),
                (5, 'alumno_ruiz',  '2026-08-07', 1)
            ) AS att(curso_num, user_name, fecha, presente)
            INNER JOIN [dbo].[Cursos] c ON c.id = att.curso_num
            INNER JOIN [dbo].[Users] u ON u.user_name = att.user_name;
        END;

        -- =============================================
        -- 15. Registrar migracion aplicada
        -- =============================================
        INSERT INTO [dbo].[SchemaVersions] (Version, ScriptName, AppliedAt)
        VALUES ('1.1.0', '03_v1.1.0_AulasCursosInscripciones.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.1.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.1.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.1.0 ya fue aplicada anteriormente.';
END;
