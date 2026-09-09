use Trabajo_Final;

-- =============================================
-- SEED v1.0.0: Datos iniciales
-- =============================================

-- Hash BCrypt para password "123": $2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy

-- =============================================
-- Roles
-- =============================================
INSERT INTO [dbo].[Roles] (name) VALUES
('Admin'),
('Profesor'),
('Alumno'),
('Coordinador');

-- =============================================
-- Permisos (cada uno = 1 formulario)
-- =============================================
INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
('FORM_USER_MGMT',    'ABM Usuarios',       'Formulario de gestion de usuarios', 0),
('FORM_ROLE_MGMT',    'ABM Roles',          'Formulario de gestion de roles', 0),
('FORM_COMPLAINTS',   'Quejas',             'Formulario de quejas', 0),
('FORM_REPORTS',      'Reportes',           'Formulario de reportes', 0),
('FORM_CHANGE_PASS',  'Cambiar Contrasena', 'Formulario de cambio de contrasena', 1),
('FORM_PREFERENCES',  'Preferencias',       'Formulario de preferencias/idioma', 1),
('FORM_CURSO_MGMT',   'ABM Cursos',         'Formulario de gestion de cursos', 0);

-- =============================================
-- Asignación de permisos a roles
-- =============================================

-- Admin (1): todos los permisos
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 1, id FROM [dbo].[Permissions];

-- Profesor (2): gestionar usuarios, cursos, quejas y reportes
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 2, id FROM [dbo].[Permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_CURSO_MGMT');

-- Alumno (3): solo quejas
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 3, id FROM [dbo].[Permissions] WHERE name IN
('FORM_COMPLAINTS');

-- Coordinador (4): todos los ABMs
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 4, id FROM [dbo].[Permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_CURSO_MGMT');

-- =============================================
-- Usuarios (contraseña: 123 para todos)
-- =============================================

-- Admin (rol 1)
INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
('admin',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System'),
('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System');

-- Coordinadores (rol 4)
INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
('coord_maria',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
('coord_carlos',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System'),
('coord_laura',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System');

-- Docentes / Profesores (rol 2)
INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
('prof_garcia',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
('prof_lopez',    '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
('prof_martinez', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
('prof_rodriguez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
('prof_fernandez','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System');

-- Alumnos (rol 3)
INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
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
-- Aulas
-- =============================================
INSERT INTO [dbo].[Aulas] (Nombre, Capacidad, Is_Active, Created_At, Last_Update) VALUES
('Aula 101 - Goiescalza',   30, 1, GETDATE(), GETDATE()),
('Aula 102 - Goiescalza',   30, 1, GETDATE(), GETDATE()),
('Aula 201 - Goiescalza',   25, 1, GETDATE(), GETDATE()),
('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
('Aula Magna - Goiescalza',  100, 1, GETDATE(), GETDATE());

-- =============================================
-- Cursos de idiomas (semestre actual: Ago-Dic 2026)
-- =============================================
INSERT INTO [dbo].[Cursos] (Nombre, Descripcion, Fecha_Inicio, Fecha_Fin, Aula_Id, Is_Active, Created_At, Last_Update) VALUES
('Ingles Basico A1',   'Curso de ingles para principiantes, nivel A1 del MCER', '2026-08-01', '2026-12-15', 1, 1, GETDATE(), GETDATE()),
('Ingles Intermedio B1','Curso de ingles nivel intermedio, preparacion para certificacion', '2026-08-01', '2026-12-15', 2, 1, GETDATE(), GETDATE()),
('Frances Inicial A1',  'Introduccion al frances, alfabetizacion y frases basicas', '2026-08-01', '2026-12-15', 3, 1, GETDATE(), GETDATE()),
('Portugues Basico',    'Curso de portugues para hispanohablantes, nivel inicial', '2026-08-01', '2026-12-15', 4, 1, GETDATE(), GETDATE()),
('Italiano A1',         'Curso de italiano para principiantes, conversacion y gramatica', '2026-08-01', '2026-12-15', NULL, 1, GETDATE(), GETDATE());

-- =============================================
-- Asignacion de docentes a cursos (N:N)
-- =============================================

-- Ingles Basico A1 -> prof_garcia (id 7) y prof_lopez (id 8)
INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
(1, 7, 1, GETDATE()),
(1, 8, 1, GETDATE());

-- Ingles Intermedio B1 -> prof_martinez (id 9)
INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
(2, 9, 1, GETDATE());

-- Frances Inicial A1 -> prof_rodriguez (id 10)
INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
(3, 10, 1, GETDATE());

-- Portugues Basico -> prof_fernandez (id 11) y prof_garcia (id 7)
INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
(4, 11, 1, GETDATE()),
(4, 7, 1, GETDATE());

-- Italiano A1 -> prof_lopez (id 8)
INSERT INTO [dbo].[CursoDocentes] (Curso_Id, Docente_Id, Is_Active, Created_At) VALUES
(5, 8, 1, GETDATE());

-- =============================================
-- Registrar migraciones aplicadas
-- =============================================
INSERT INTO [dbo].[SchemaVersions] (Version, ScriptName, AppliedAt)
VALUES ('1.0.0', '02_v1.0.0_SeedData.sql', GETDATE());
