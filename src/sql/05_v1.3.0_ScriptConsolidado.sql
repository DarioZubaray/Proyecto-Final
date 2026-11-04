-- ============================================================
-- SCRIPT CONSOLIDADO v1.3.0
-- Instalacion completa de la base DarioZubaray_TF en un solo paso:
--   1. Crear la base si no existe
--   2. Eliminar todas las tablas (purge)
--   3. Crear el esquema completo (estado final v1.3.0)
--   4. Insertar datos iniciales, seed academico y backfill DVH
--   5. Registrar las versiones 1.0.0 y 1.3.0 en schema_versions
-- Equivale a ejecutar 01 + 02 + 03 en un solo script.
-- ============================================================

USE master;
GO

-- =============================================
-- 1. Crear base de datos si no existe
-- =============================================
IF DB_ID(N'DarioZubaray_TF') IS NULL
    CREATE DATABASE DarioZubaray_TF;
GO

USE DarioZubaray_TF;
GO

BEGIN TRANSACTION;

BEGIN TRY

    -- =============================================
    -- 2. PURGE: eliminar todas las tablas
    --    (orden inverso: primero las hijas)
    -- =============================================
    IF OBJECT_ID('dbo.grades', 'U') IS NOT NULL DROP TABLE dbo.grades;
    IF OBJECT_ID('dbo.evaluations', 'U') IS NOT NULL DROP TABLE dbo.evaluations;
    IF OBJECT_ID('dbo.tickets', 'U') IS NOT NULL DROP TABLE dbo.tickets;
    IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL DROP TABLE dbo.attendance;
    IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL DROP TABLE dbo.course_students;
    IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL DROP TABLE dbo.course_teachers;
    IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL DROP TABLE dbo.courses;
    IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL DROP TABLE dbo.classrooms;
    IF OBJECT_ID('dbo.activity_logs', 'U') IS NOT NULL DROP TABLE dbo.activity_logs;
    IF OBJECT_ID('dbo.students', 'U') IS NOT NULL DROP TABLE dbo.students;
    IF OBJECT_ID('dbo.role_hierarchy', 'U') IS NOT NULL DROP TABLE dbo.role_hierarchy;
    IF OBJECT_ID('dbo.role_permissions', 'U') IS NOT NULL DROP TABLE dbo.role_permissions;
    IF OBJECT_ID('dbo.schema_versions', 'U') IS NOT NULL DROP TABLE dbo.schema_versions;
    IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users;
    IF OBJECT_ID('dbo.permissions', 'U') IS NOT NULL DROP TABLE dbo.permissions;
    IF OBJECT_ID('dbo.roles', 'U') IS NOT NULL DROP TABLE dbo.roles;

    -- =============================================
    -- 3. CREATE: esquema base (01) + columna DVH
    -- =============================================

    -- 3.1 Roles
    CREATE TABLE [dbo].[roles] (
        [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [name] NVARCHAR(100)  NOT NULL UNIQUE
    );

    -- 3.2 Permisos
    CREATE TABLE [dbo].[permissions] (
        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [name]        NVARCHAR(100)  NOT NULL UNIQUE,
        [label]       NVARCHAR(100)  NOT NULL,
        [description] NVARCHAR(256)  NULL,
        [is_system]   BIT            NOT NULL DEFAULT 0
    );

    -- 3.3 Relacion Roles <-> Permisos (N:N)
    CREATE TABLE [dbo].[role_permissions] (
        [role_id]       INT NOT NULL,
        [permission_id] INT NOT NULL,
        PRIMARY KEY ([role_id], [permission_id]),
        CONSTRAINT fk_role_permissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[roles]([id]),
        CONSTRAINT fk_role_permissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[permissions]([id])
    );

    -- 3.4 Jerarquia de roles
    CREATE TABLE [dbo].[role_hierarchy] (
        [parent_role_id] INT NOT NULL,
        [child_role_id]  INT NOT NULL,
        PRIMARY KEY ([parent_role_id], [child_role_id]),
        CONSTRAINT fk_role_hierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[roles]([id]),
        CONSTRAINT fk_role_hierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[roles]([id])
    );

    -- 3.5 Usuarios (con DVH, estado final)
    CREATE TABLE [dbo].[users] (
        [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [user_name]     NVARCHAR(100)  NOT NULL,
        [password_hash] NVARCHAR(256)  NOT NULL,
        [is_active]     BIT            NOT NULL DEFAULT 1,
        [retries_count] INT            NOT NULL DEFAULT 0,
        [last_update]   DATETIME       NOT NULL,
        [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
        [role_id]       INT            NULL,
        [language]      NVARCHAR(10)   NOT NULL DEFAULT 'es',
        [theme]         NVARCHAR(20)   NOT NULL DEFAULT 'System',
        [dvh]           NVARCHAR(64)   NULL,
        CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[roles]([id])
    );
    ALTER TABLE [dbo].[users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);

    -- 3.6 Bitacora de actividad
    CREATE TABLE [dbo].[activity_logs] (
        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [user_id]     INT            NOT NULL,
        [action]      NVARCHAR(64)   NOT NULL,
        [form_name]   NVARCHAR(100)  NULL,
        [description] NVARCHAR(256)  NULL,
        [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_activity_logs_users FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
    );
    CREATE INDEX ix_activity_logs_user_created ON [dbo].[activity_logs] ([user_id], [created_at] DESC);

    -- 3.7 Tabla de versiones de migracion
    CREATE TABLE [dbo].[schema_versions] (
        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [version]     NVARCHAR(20)   NOT NULL,
        [script_name] NVARCHAR(200)  NOT NULL,
        [applied_at]  DATETIME       NOT NULL DEFAULT GETDATE()
    );

    -- =============================================
    -- 4. CREATE: tablas academicas y de gestion
    --    (estado final; students antes que las
    --    tablas que la referencian por FK)
    -- =============================================

    -- 4.1 Aulas
    CREATE TABLE [dbo].[classrooms] (
        [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [name]         NVARCHAR(100)  NOT NULL,
        [capacity]     INT            NOT NULL,
        [is_active]    BIT            NOT NULL DEFAULT 1,
        [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
        [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
    );

    -- 4.2 Cursos
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

    -- 4.3 Docentes por curso (N:N)
    CREATE TABLE [dbo].[course_teachers] (
        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
        [course_id]    INT      NOT NULL,
        [teacher_id]   INT      NOT NULL,
        [is_active]    BIT      NOT NULL DEFAULT 1,
        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_course_teachers_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
        CONSTRAINT fk_course_teachers_users   FOREIGN KEY ([teacher_id]) REFERENCES [dbo].[users]([id])
    );

    -- 4.4 Alumnos (entidad propia; no son usuarios)
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

    -- 4.5 Inscripciones (FK a students)
    CREATE TABLE [dbo].[course_students] (
        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
        [course_id]    INT      NOT NULL,
        [student_id]   INT      NOT NULL,
        [is_active]    BIT      NOT NULL DEFAULT 1,
        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_course_students_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
        CONSTRAINT fk_course_students_students FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id])
    );

    -- 4.6 Asistencia (FK a students)
    CREATE TABLE [dbo].[attendance] (
        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
        [course_id]    INT      NOT NULL,
        [student_id]   INT      NOT NULL,
        [date]         DATE     NOT NULL,
        [is_present]   BIT      NOT NULL DEFAULT 0,
        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_attendance_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
        CONSTRAINT fk_attendance_students FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]),
        CONSTRAINT uq_attendance_unique  UNIQUE ([course_id], [student_id], [date])
    );

    -- 4.7 Tickets
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

    -- 4.8 Evaluaciones
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

    -- 4.9 Notas (0 a 10, unica por alumno y evaluacion)
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

    -- =============================================
    -- 5. INSERT: roles (ids fijos del seed; sin
    --    rol Alumno, que elimina v1.5.0)
    -- =============================================
    SET IDENTITY_INSERT [dbo].[roles] ON;

    INSERT INTO [dbo].[roles] ([id], [name]) VALUES
    (1, 'Admin'),
    (2, 'Profesor'),
    (4, 'Coordinador'),
    (5, 'Director');

    SET IDENTITY_INSERT [dbo].[roles] OFF;

    -- =============================================
    -- 6. INSERT: permisos (15, estado final)
    -- =============================================
    INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
    ('FORM_USER_MGMT',        'ABM Usuarios',            'Formulario de gestion de usuarios',                       0),
    ('FORM_ROLE_MGMT',        'ABM Roles',               'Formulario de gestion de roles',                          0),
    ('FORM_COMPLAINTS',       'Quejas',                  'Formulario de quejas',                                    0),
    ('FORM_REPORTS',          'Reportes',                'Formulario de reportes',                                  0),
    ('FORM_CHANGE_PASS',      'Cambiar Contrasena',      'Formulario de cambio de contrasena',                      1),
    ('FORM_PREFERENCES',      'Preferencias',            'Formulario de preferencias/idioma',                       1),
    ('FORM_CURSO_MGMT',       'ABM Cursos',              'Formulario de gestion de cursos',                         0),
    ('FORM_INSCRIPCION_MGMT', 'Inscripciones',           'Gestion de inscripciones de alumnos a cursos',            0),
    ('FORM_ASISTENCIA_MGMT',  'Asistencia',              'Registro de asistencia de alumnos en clases',             0),
    ('FORM_ASISTENCIA_VIEW',  'Ver Asistencia',          'Consulta de asistencia de alumnos',                       0),
    ('FORM_TICKET_MGMT',      'Gestion de Tickets',      'Listar y cerrar tickets de todos los usuarios',           0),
    ('FORM_STUDENT_MGMT',     'ABM Alumnos',             'Gestion de alumnos (entidad, no usuarios)',               0),
    ('FORM_EVAL_MGMT',        'Evaluaciones',            'Registro de evaluaciones y notas de los cursos del docente', 0),
    ('FORM_REPORT_DETAIL',    'Informes Personalizados', 'Generacion y exportacion de informes academicos por curso', 0),
    ('FORM_BACKUP_MGMT',      'Respaldo y Restauracion', 'Generacion de respaldos y restauracion de la base de datos', 0);

    -- =============================================
    -- 7. INSERT: asignacion final de permisos por
    --    rol (mismo estado que 01 + 02 + 03)
    -- =============================================

    -- Admin (1): todos los permisos
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 1, id FROM [dbo].[permissions];

    -- Profesor (2): operador de asistencia/evaluaciones/quejas
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 2, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_COMPLAINTS', 'FORM_ASISTENCIA_MGMT', 'FORM_EVAL_MGMT');

    -- Coordinador (4): tareas diarias (sin ABM de usuarios/roles ni reportes)
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 4, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_VIEW', 'FORM_COMPLAINTS', 'FORM_TICKET_MGMT', 'FORM_STUDENT_MGMT');

    -- Director (5): solo consulta de reportes
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 5, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_REPORTS', 'FORM_REPORT_DETAIL');

    -- =============================================
    -- 8. INSERT: usuarios (contrasena: 123 para
    --    todos, excepto admin: admin)
    -- =============================================

    -- Admin (rol 1) - exento de bloqueo por reintentos, requiere 2FA (codigo: 123)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('admin',  '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System'),
    ('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System');

    -- Coordinadores (rol 4)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('coord_maria',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
    ('coord_carlos',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
    ('coord_laura',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System');

    -- Docentes / Profesores (rol 2)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('prof_garcia',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_lopez',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_martinez', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_rodriguez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('prof_fernandez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System');

    -- =============================================
    -- 9. INSERT: alumnos (entidad students; los
    --    mismos 15 registros y IDs 11-25 que la
    --    migracion v1.5.0 sobre el seed de usuarios)
    -- =============================================
    SET IDENTITY_INSERT [dbo].[students] ON;

    INSERT INTO [dbo].[students] ([id], [first_name], [last_name], [document], [email], [phone], [is_active], [created_at], [last_update]) VALUES
    (11, 'alumno', 'perez',    'DNI-00000011', NULL, NULL, 1, GETDATE(), GETDATE()),
    (12, 'alumno', 'gomez',    'DNI-00000012', NULL, NULL, 1, GETDATE(), GETDATE()),
    (13, 'alumno', 'diaz',     'DNI-00000013', NULL, NULL, 1, GETDATE(), GETDATE()),
    (14, 'alumno', 'morales',  'DNI-00000014', NULL, NULL, 1, GETDATE(), GETDATE()),
    (15, 'alumno', 'torres',   'DNI-00000015', NULL, NULL, 1, GETDATE(), GETDATE()),
    (16, 'alumno', 'ramos',    'DNI-00000016', NULL, NULL, 1, GETDATE(), GETDATE()),
    (17, 'alumno', 'silva',    'DNI-00000017', NULL, NULL, 1, GETDATE(), GETDATE()),
    (18, 'alumno', 'castro',   'DNI-00000018', NULL, NULL, 1, GETDATE(), GETDATE()),
    (19, 'alumno', 'ruiz',     'DNI-00000019', NULL, NULL, 1, GETDATE(), GETDATE()),
    (20, 'alumno', 'alvarez',  'DNI-00000020', NULL, NULL, 1, GETDATE(), GETDATE()),
    (21, 'alumno', 'romero',   'DNI-00000021', NULL, NULL, 1, GETDATE(), GETDATE()),
    (22, 'alumno', 'sanchez',  'DNI-00000022', NULL, NULL, 1, GETDATE(), GETDATE()),
    (23, 'alumno', 'herrera',  'DNI-00000023', NULL, NULL, 1, GETDATE(), GETDATE()),
    (24, 'alumno', 'patricio', 'DNI-00000024', NULL, NULL, 1, GETDATE(), GETDATE()),
    (25, 'alumno', 'soto',     'DNI-00000025', NULL, NULL, 1, GETDATE(), GETDATE());

    SET IDENTITY_INSERT [dbo].[students] OFF;

    -- =============================================
    -- 10. INSERT: seed academico
    -- =============================================

    -- 10.1 Aulas
    INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
    ('Aula 101 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
    ('Aula 102 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
    ('Aula 201 - Goiescalza',      25, 1, GETDATE(), GETDATE()),
    ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
    ('Aula Magna - Goiescalza',   100, 1, GETDATE(), GETDATE());

    -- 10.2 Cursos con horario
    INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
    ('Ingles Basico A1',     'Curso de ingles para principiantes, nivel A1 del MCER',             '2026-08-01', '2026-12-15', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
    ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion',  '2026-08-01', '2026-12-15', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
    ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',         '2026-08-01', '2026-12-15', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
    ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',           '2026-08-01', '2026-12-15', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
    ('Italiano A1',          'Curso de italiano para principantes, conversacion y gramatica',     '2026-08-01', '2026-12-15', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());

    -- 10.3 Docentes por curso
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

    -- 10.4 Inscripciones (los alumnos son la entidad students)
    INSERT INTO [dbo].[course_students] (course_id, student_id, is_active, created_at)
    SELECT c.id, s.id, 1, GETDATE()
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
    ) AS e(course_num, student_name)
    INNER JOIN [dbo].[courses] c ON c.id = e.course_num
    INNER JOIN [dbo].[students] s ON (s.[first_name] + '_' + s.[last_name]) = e.student_name;

    -- 10.5 Asistencia
    INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
    SELECT c.id, s.id, a.date, a.is_present, GETDATE()
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
    ) AS a(course_num, student_name, date, is_present)
    INNER JOIN [dbo].[courses] c ON c.id = a.course_num
    INNER JOIN [dbo].[students] s ON (s.[first_name] + '_' + s.[last_name]) = a.student_name;

    -- =============================================
    -- 11. Registrar versiones
    -- =============================================
    INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at) VALUES
    ('1.0.0', '05_v1.3.0_ScriptConsolidado.sql', GETDATE()),
    ('1.3.0', '05_v1.3.0_ScriptConsolidado.sql', GETDATE());

    -- =============================================
    -- 12. Confirmar
    -- =============================================
    COMMIT TRANSACTION;

    PRINT 'Script consolidado aplicado exitosamente. Base DarioZubaray_TF lista con esquema v1.3.0.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error al aplicar el script consolidado: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO

-- =============================================
-- DVH: backfill del digito verificador (lote
-- aparte). SQL Server compila cada lote completo
-- contra el esquema previo a su ejecucion: un
-- UPDATE que referencia [dvh] dentro del lote de
-- instalacion fallaria cuando la base previa ya
-- tenia la tabla users sin esa columna. Aqui la
-- columna ya existe (la crea este script en el
-- lote anterior).
--
-- Replica la serializacion canonica de la app
-- (BLL.DVHHelperBLL): columnas menos [dvh], en
-- orden de columna, unidas por coma; fechas estilo
-- 126; role_id NULL -> ''; SHA2_256 sobre NVARCHAR
-- (bytes UTF-16LE, igual que Encoding.Unicode).
-- =============================================
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL
BEGIN
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
END
GO
