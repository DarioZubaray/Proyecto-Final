-- ============================================================
-- SCRIPT CONSOLIDADO v1.1.0
-- Instalacion completa de la base DarioZubaray_TF en un solo paso:
--   1. Crear la base si no existe
--   2. Eliminar todas las tablas (purge)
--   3. Crear el esquema completo (v1.0.0 + v1.1.0)
--   4. Insertar datos iniciales y seed de la migracion v1.1.0
--   5. Registrar versiones en schema_versions
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
    -- =============================================
    IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL DROP TABLE dbo.attendance;
    IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL DROP TABLE dbo.course_students;
    IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL DROP TABLE dbo.course_teachers;
    IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL DROP TABLE dbo.courses;
    IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL DROP TABLE dbo.classrooms;
    IF OBJECT_ID('dbo.activity_logs', 'U') IS NOT NULL DROP TABLE dbo.activity_logs;
    IF OBJECT_ID('dbo.role_hierarchy', 'U') IS NOT NULL DROP TABLE dbo.role_hierarchy;
    IF OBJECT_ID('dbo.role_permissions', 'U') IS NOT NULL DROP TABLE dbo.role_permissions;
    IF OBJECT_ID('dbo.schema_versions', 'U') IS NOT NULL DROP TABLE dbo.schema_versions;
    IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users;
    IF OBJECT_ID('dbo.permissions', 'U') IS NOT NULL DROP TABLE dbo.permissions;
    IF OBJECT_ID('dbo.roles', 'U') IS NOT NULL DROP TABLE dbo.roles;

    -- =============================================
    -- 3. CREATE: esquema base v1.0.0
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

    -- 3.5 Usuarios
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
    -- 4. INSERT: datos iniciales v1.0.0
    -- =============================================

    -- Hash BCrypt para password "123": $2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy

    -- 4.1 Roles
    INSERT INTO [dbo].[roles] (name) VALUES
    ('Admin'),
    ('Profesor'),
    ('Alumno'),
    ('Coordinador');

    -- 4.2 Permisos (v1.0.0 + v1.1.0)
    INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
    ('FORM_USER_MGMT',        'ABM Usuarios',       'Formulario de gestion de usuarios',      0),
    ('FORM_ROLE_MGMT',        'ABM Roles',          'Formulario de gestion de roles',         0),
    ('FORM_COMPLAINTS',       'Quejas',             'Formulario de quejas',                   0),
    ('FORM_REPORTS',          'Reportes',           'Formulario de reportes',                 0),
    ('FORM_CHANGE_PASS',      'Cambiar Contrasena', 'Formulario de cambio de contrasena',     1),
    ('FORM_PREFERENCES',      'Preferencias',       'Formulario de preferencias/idioma',      1),
    ('FORM_CURSO_MGMT',       'ABM Cursos',         'Formulario de gestion de cursos',        0),
    ('FORM_INSCRIPCION_MGMT', 'Inscripciones',      'Gestion de inscripciones de alumnos a cursos', 0),
    ('FORM_ASISTENCIA_MGMT',  'Asistencia',         'Registro de asistencia de alumnos en clases',  0),
    ('FORM_ASISTENCIA_VIEW',  'Ver Asistencia',     'Consulta de asistencia de alumnos',      0);

    -- 4.3 Asignacion de permisos a roles (estado final)

    -- Admin (1): todos
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 1, id FROM [dbo].[permissions];

    -- Profesor (2)
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 2, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_MGMT');

    -- Alumno (3)
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 3, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_COMPLAINTS', 'FORM_INSCRIPCION_MGMT');

    -- Coordinador (4)
    INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
    SELECT 4, id FROM [dbo].[permissions] WHERE name IN
    ('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_ASISTENCIA_VIEW');

    -- 4.4 Usuarios (contrasena: 123 para todos)

    -- Admin (rol 1)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('admin',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System'),
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

    -- Alumnos (rol 3)
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('alumno_perez',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_gomez',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_diaz',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_morales',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_torres',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_ramos',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_silva',     '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_castro',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_ruiz',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_alvarez',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_romero',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_sanchez',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_herrera',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_patricio',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('alumno_soto',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es', 'System');

    -- =============================================
    -- 5. CREATE: tablas v1.1.0 (aulas, cursos, inscripciones, asistencia)
    -- =============================================

    -- 5.1 Classrooms
    CREATE TABLE [dbo].[classrooms] (
        [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
        [name]         NVARCHAR(100)  NOT NULL,
        [capacity]     INT            NOT NULL,
        [is_active]    BIT            NOT NULL DEFAULT 1,
        [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
        [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
    );

    -- 5.2 Courses
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

    -- 5.3 Course Teachers (N:N)
    CREATE TABLE [dbo].[course_teachers] (
        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
        [course_id]    INT      NOT NULL,
        [teacher_id]   INT      NOT NULL,
        [is_active]    BIT      NOT NULL DEFAULT 1,
        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_course_teachers_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
        CONSTRAINT fk_course_teachers_users   FOREIGN KEY ([teacher_id]) REFERENCES [dbo].[users]([id])
    );

    -- 5.4 Course Students (enrollments)
    CREATE TABLE [dbo].[course_students] (
        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
        [course_id]    INT      NOT NULL,
        [student_id]   INT      NOT NULL,
        [is_active]    BIT      NOT NULL DEFAULT 1,
        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT fk_course_students_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
        CONSTRAINT fk_course_students_users   FOREIGN KEY ([student_id]) REFERENCES [dbo].[users]([id])
    );

    -- 5.5 Attendance
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

    -- =============================================
    -- 6. INSERT: seed v1.1.0
    -- =============================================

    -- 6.1 Classrooms
    INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
    ('Aula 101 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
    ('Aula 102 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
    ('Aula 201 - Goiescalza',      25, 1, GETDATE(), GETDATE()),
    ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
    ('Aula Magna - Goiescalza',   100, 1, GETDATE(), GETDATE());

    -- 6.2 Courses with schedules
    INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
    ('Ingles Basico A1',     'Curso de ingles para principiantes, nivel A1 del MCER',             '2026-08-01', '2026-12-15', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
    ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion',  '2026-08-01', '2026-12-15', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
    ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',         '2026-08-01', '2026-12-15', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
    ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',           '2026-08-01', '2026-12-15', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
    ('Italiano A1',          'Curso de italiano para principantes, conversacion y gramatica',     '2026-08-01', '2026-12-15', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());

    -- 6.3 Course Teachers
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

    -- 6.4 Course Students (enrollments)
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

    -- 6.5 Attendance records
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

    -- =============================================
    -- 7. Registrar versiones de migracion
    -- =============================================
    INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at) VALUES
    ('1.0.0', '06_v1.1.0_ScriptConsolidado.sql', GETDATE()),
    ('1.1.0', '06_v1.1.0_ScriptConsolidado.sql', GETDATE());

    -- =============================================
    -- 8. Confirmar
    -- =============================================
    COMMIT TRANSACTION;

    PRINT 'Script consolidado aplicado exitosamente. Base DarioZubaray_TF lista con esquema v1.1.0.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error al aplicar el script consolidado: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
GO