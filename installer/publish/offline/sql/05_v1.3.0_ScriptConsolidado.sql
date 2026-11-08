-- ============================================================
-- SCRIPT CONSOLIDADO v1.3.0 (atajo dev / demo)
-- Instalacion completa de la base DarioZubaray_TF en un solo paso:
-- 
--   1. Crear la base si no existe
--   2. Eliminar todas las tablas (purge) -- destructive
--   3. Crear el esquema completo (estado final v1.3.0)
--   4. Insertar datos iniciales, seed academico y backfill DVH
--   5. Registrar las versiones 1.0.0 y 1.3.0 en schema_versions
--
-- Equivale a ejecutar 01 + 02 + 03 en un solo script.
--
-- NOTA: no es el script del instalador. El instalador ejecuta
-- la cadena 01 -> 03 (sin datos demo). Este consolidado es un
-- atajo para desarrollo/demostracion y BORRA el contenido
-- existente de la base.
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
    -- 2. PURGE: eliminar todas las tablas (orden inverso: primero las hijas)
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

    -- 4.3 Profesores por curso (N:N)
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
    -- 5. INSERT: roles
    -- =============================================
    SET IDENTITY_INSERT [dbo].[roles] ON;

    INSERT INTO [dbo].[roles] ([id], [name]) VALUES
    (1, 'Admin'),
    (2, 'Coordinador'),
    (3, 'Profesor'),
    (4, 'Director');

    SET IDENTITY_INSERT [dbo].[roles] OFF;

    -- =============================================
    -- 6. INSERT: permisos
    -- =============================================
    INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
    ('FORM_PREFERENCES',      'Preferencias',            'Formulario de idioma/tema',                                   0),
    ('FORM_CHANGE_PASS',      'Cambiar Contraseña',      'Formulario de cambio de contraseña',                          0),
    ('FORM_ACTIVITY_HISTORY', 'Historial de Actividad',  'Bitacora de actividad del usuario',                           0),
    ('FORM_LOGOUT',           'Cerrar Sesion',           'Cierre de sesion desde el menu principal',                    0),

    ('FORM_USER_MGMT',        'ABM Usuarios',            'Formulario de gestion de usuarios',                           0),
    ('FORM_ROLE_MGMT',        'ABM Roles',               'Formulario de gestion de roles',                              0),
    ('FORM_STUDENT_MGMT',     'ABM Alumnos',             'Formulario de gestion de alumnos',                            0),
    ('FORM_CLASSROOM_MGMT',   'ABM Aulas',               'Formulario de gestion de aulas',                              0),
    ('FORM_CURSO_MGMT',       'ABM Cursos',              'Formulario de gestion de cursos',                             0),
    ('FORM_INSCRIPCION_MGMT', 'ABM Inscripciones',       'Formulario de gestion de inscripciones de alumnos a cursos',  0),
    ('FORM_BACKUP_MGMT',      'Respaldo y Restauracion', 'Generacion de respaldos y restauración de la base de datos',  0),
    
    ('FORM_TICKET_MGMT',      'Gestion de Tickets',      'Listar y cerrar tickets de todos los usuarios',               0),
    ('FORM_TICKET_CREATE',    'Crear Tickets',           'Alta de tickets de soporte (Coordinador y Profesor)',         0),
    
    ('FORM_ASISTENCIA_MGMT',  'Asistencia',              'Registro de asistencia de alumnos en clases',                 0),
    ('FORM_ASISTENCIA_VIEW',  'Ver Asistencia',          'Consulta de asistencia de alumnos',                           0),
    ('FORM_EVAL_MGMT',        'Evaluaciones',            'Registro de evaluaciones y notas de los cursos del profesor', 0),
    
    ('FORM_REPORTS',          'Reportes',                'Formulario de reportes',                                      0),
    ('FORM_REPORT_DETAIL',    'Informes Personalizados', 'Generacion y exportacion de informes academicos por curso',   0),
    
    ('FORM_ABOUT',            'Acerca de',               'Ventana acerca de la aplicacion',                             0);

    -- =============================================
    -- 7. INSERT: asignacion de permisos por rol
    -- =============================================

    -- Admin (1): todos los permisos
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 1, id FROM [dbo].[permissions];

    -- Coordinador (2): tareas diarias (sin ABM de usuarios/roles ni reportes)
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 2, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_STUDENT_MGMT', 'FORM_CURSO_MGMT', 'FORM_CLASSROOM_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_VIEW', 'FORM_TICKET_MGMT', 'FORM_TICKET_CREATE',
    'FORM_PREFERENCES', 'FORM_CHANGE_PASS', 'FORM_ACTIVITY_HISTORY', 'FORM_LOGOUT', 'FORM_ABOUT');

    -- Profesor (3): operador de asistencia/evaluaciones + creacion de tickets
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 3, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_ASISTENCIA_MGMT', 'FORM_EVAL_MGMT', 'FORM_TICKET_CREATE',
    'FORM_PREFERENCES', 'FORM_CHANGE_PASS', 'FORM_ACTIVITY_HISTORY', 'FORM_LOGOUT', 'FORM_ABOUT');

    -- Director (4): solo consulta de reportes
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 4, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_REPORTS', 'FORM_REPORT_DETAIL',
    'FORM_PREFERENCES', 'FORM_CHANGE_PASS', 'FORM_ACTIVITY_HISTORY', 'FORM_LOGOUT', 'FORM_ABOUT');


    -- =============================================
    -- 8. INSERT: usuarios (contrasena: 123 para todos, excepto admin: admin)
    -- =============================================

    -- Admin (rol 1) - exento de bloqueo por reintentos, requiere 2FA (codigo: 123)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('admin',  '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System'),
    ('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System');

    -- Coordinadores (rol 2)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('coord1', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('coord2', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('coord3', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System');

    -- Profesores (rol 3)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('prof_brooks',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_omh',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_conway',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_pareto',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_amdahl',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_kirchhoff','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_murphy',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_newton',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System');

    -- Director (rol 4)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('dir_schwarzenegger', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System');

    -- Usuario bloqueados
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('user_blocked',  '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G', 0, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('user_1retry',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 1, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('user_2retry',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 2, GETDATE(), GETDATE(), 1, 'es', 'System');

    -- =============================================
    -- 9. INSERT: alumnos 
    -- =============================================
    SET IDENTITY_INSERT [dbo].[students] ON;

    INSERT INTO [dbo].[students] ([id], [first_name], [last_name], [document], [email], [phone], [is_active], [created_at], [last_update]) VALUES
    (1, 'alumno', 'juan',        'DNI-10200301', NULL, NULL, 1, GETDATE(), GETDATE()),
    (2, 'alumno', 'matias',      'DNI-10200302', NULL, NULL, 1, GETDATE(), GETDATE()),
    (3, 'alumno', 'pedro',       'DNI-10200303', NULL, NULL, 1, GETDATE(), GETDATE()),
    (4, 'alumno', 'tomás',       'DNI-10200304', NULL, NULL, 1, GETDATE(), GETDATE()),
    (5, 'alumno', 'gerardo',     'DNI-10200305', NULL, NULL, 1, GETDATE(), GETDATE()),
    (6, 'alumno', 'lucia',       'DNI-10200306', NULL, NULL, 1, GETDATE(), GETDATE()),
    (7, 'alumno', 'silvia',      'DNI-10200307', NULL, NULL, 1, GETDATE(), GETDATE()),
    (8, 'alumno', 'romina',      'DNI-10200308', NULL, NULL, 1, GETDATE(), GETDATE()),
    (9, 'alumno', 'ana',         'DNI-10200309', NULL, NULL, 1, GETDATE(), GETDATE()),
    (10, 'alumno', 'maria',      'DNI-10200310', NULL, NULL, 1, GETDATE(), GETDATE()),
    (11, 'alumno', 'antonela',   'DNI-10200311', NULL, NULL, 1, GETDATE(), GETDATE()),
    (12, 'alumno', 'julieta',    'DNI-10200312', NULL, NULL, 1, GETDATE(), GETDATE()),
    (13, 'alumno', 'agustín',    'DNI-10200313', NULL, NULL, 1, GETDATE(), GETDATE()),
    (14, 'alumno', 'benjamín',   'DNI-10200314', NULL, NULL, 1, GETDATE(), GETDATE()),
    (15, 'alumno', 'enzo',       'DNI-10200315', NULL, NULL, 1, GETDATE(), GETDATE());

    SET IDENTITY_INSERT [dbo].[students] OFF;

    -- =============================================
    -- 10. INSERT: seed academico
    -- =============================================

    -- 10.1 Aulas
    INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
    ('Aula 101',      30, 1, GETDATE(), GETDATE()),
    ('Aula 102',      30, 1, GETDATE(), GETDATE()),
    ('Aula 201',      25, 1, GETDATE(), GETDATE()),
    ('Laboratorio 1', 20, 1, GETDATE(), GETDATE()),
    ('Aula 202',     100, 1, GETDATE(), GETDATE());

    -- 10.2 Cursos con horario
    INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
    ('Ingles A1',   'Curso de ingles para principiantes',        '2026-08-01', '2026-12-31', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
    ('Ingles B1',   'Curso de ingles nivel intermedio',          '2026-08-01', '2026-12-31', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
    ('Frances A1',  'Introduccion al frances',                   '2026-08-01', '2026-12-31', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
    ('Portugues A1','Curso de portugues para hispanohablantes',  '2026-08-01', '2026-12-31', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
    ('Italiano A1', 'Curso de italiano para principantes',       '2026-08-01', '2026-12-31', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());

    -- 10.3 Profesor por curso
    INSERT INTO [dbo].[course_teachers] (course_id, teacher_id, is_active, created_at)
    SELECT c.id, u.id, 1, GETDATE()
    FROM (VALUES
        (1, 'prof_brooks'),
        (1, 'prof_omh'),
        (2, 'prof_conway'),
        (3, 'prof_pareto'),
        (4, 'prof_amdahl'),
        (4, 'prof_kirchhoff'),
        (5, 'prof_murphy')
    ) AS t(course_num, user_name)
    INNER JOIN [dbo].[courses] c ON c.id = t.course_num
    INNER JOIN [dbo].[users] u ON u.user_name = t.user_name;

    -- 10.4 Inscripciones
    INSERT INTO [dbo].[course_students] (course_id, student_id, is_active, created_at)
    SELECT c.id, s.id, 1, GETDATE()
    FROM (VALUES
        (1, 'alumno_juan'),
        (1, 'alumno_matias'),
        (1, 'alumno_pedro'),
        (1, 'alumno_tomás'),
        (1, 'alumno_gerardo'),
        (2, 'alumno_lucia'),
        (2, 'alumno_silvia'),
        (2, 'alumno_romina'),
        (3, 'alumno_ana'),
        (3, 'alumno_maria'),
        (3, 'alumno_antonela'),
        (3, 'alumno_julieta'),
        (4, 'alumno_agustín'),
        (4, 'alumno_benjamín'),
        (4, 'alumno_enzo'),
        (5, 'alumno_juan'),
        (5, 'alumno_maria')
    ) AS e(course_num, student_name)
    INNER JOIN [dbo].[courses] c ON c.id = e.course_num
    INNER JOIN [dbo].[students] s ON (s.[first_name] + '_' + s.[last_name]) = e.student_name;

    -- 10.5 Asistencia
    INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
    SELECT c.id, s.id, a.date, a.is_present, GETDATE()
    FROM (VALUES
        (1, 'alumno_juan',     '2026-08-03', 1),
        (1, 'alumno_matias',   '2026-08-03', 1),
        (1, 'alumno_pedro',    '2026-08-03', 0),
        (1, 'alumno_tomás',    '2026-08-03', 1),
        (1, 'alumno_gerardo',  '2026-08-03', 1),
        (1, 'alumno_juan',     '2026-08-10', 1),
        (1, 'alumno_matias',   '2026-08-10', 0),
        (1, 'alumno_pedro',    '2026-08-10', 1),
        (1, 'alumno_tomás',    '2026-08-10', 1),
        (1, 'alumno_gerardo',  '2026-08-10', 1),
        (2, 'alumno_lucia',    '2026-08-05', 1),
        (2, 'alumno_silvia',   '2026-08-05', 1),
        (2, 'alumno_romina',   '2026-08-05', 0),
        (2, 'alumno_lucia',    '2026-08-12', 1),
        (2, 'alumno_silvia',   '2026-08-12', 0),
        (2, 'alumno_romina',   '2026-08-12', 1),
        (3, 'alumno_ana',      '2026-08-04', 1),
        (3, 'alumno_maria',    '2026-08-04', 1),
        (3, 'alumno_antonela', '2026-08-04', 1),
        (3, 'alumno_julieta',  '2026-08-04', 0),
        (4, 'alumno_agustín',  '2026-08-06', 1),
        (4, 'alumno_benjamín', '2026-08-06', 0),
        (4, 'alumno_enzo',     '2026-08-06', 1),
        (5, 'alumno_juan',     '2026-08-07', 1),
        (5, 'alumno_ana',      '2026-08-07', 1)
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
-- DVH: backfill del digito verificador 
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
