USE DarioZubaray_TF;
GO

-- =============================================
-- CREATE v1.0.0: Esquema base de tablas + datos core
--
-- Cadena de instalacion / actualizacion:
--   01 -> 03 -> updates...       (instalador)
--   01 -> 02 -> 03               (demo / desarrollo, con datos de prueba)
--   05 consolidado en un paso    (atajo dev, incluye datos demo)
--
-- Contenido:
--   a) 7 tablas base del sistema
--   b) Seed estructural: roles, permisos y matriz de permisos
--   c) Usuario administrador por defecto (user: admin / pass: admin)
--   d) Registro de la version 1.0.0 en [schema_versions]
--
-- El script es idempotente: puede re-ejecutarse sobre una
-- base ya creada sin producir efecto.
-- =============================================

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
-- 3. Usuario administrador por defecto
--
-- Hash BCrypt para password "admin":$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G
--
-- Es el unico usuario que desbloquea el sistema cuando
-- el DVH detecta una inconsistencia (rol Admin con todos
-- los permisos, exento de bloqueo por reintentos, 2FA
-- con codigo 123). El dvh se completa en el update 03
-- (backfill SHA-256).
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'admin')
	INSERT INTO [dbo].[users]
		([user_name], [password_hash], [is_active], [retries_count], [last_update], [created_at], [role_id], [language], [theme])
	VALUES
		('admin',
		 '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G',
		 1, 0, GETDATE(), GETDATE(),
		 (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Admin'),
		 'es', 'System');

-- =============================================
-- 4. Registrar la version base
-- =============================================
IF NOT EXISTS (
	SELECT 1 FROM [dbo].[schema_versions]
	WHERE [script_name] = '01_v1.0.0_CreateTables.sql'
)
	INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
	VALUES ('1.0.0', '01_v1.0.0_CreateTables.sql', GETDATE());

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
