USE DarioZubaray_TF;

-- =============================================
-- SEED v1.0.0: Datos iniciales
-- =============================================

-- Hash BCrypt para password "123": $2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy

-- Roles
INSERT INTO [dbo].[roles] (name) VALUES
('Admin'),
('Profesor'),
('Alumno'),
('Coordinador');

-- Permisos
INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
('FORM_USER_MGMT',    'ABM Usuarios',       'Formulario de gestion de usuarios', 0),
('FORM_ROLE_MGMT',    'ABM Roles',          'Formulario de gestion de roles', 0),
('FORM_COMPLAINTS',   'Quejas',             'Formulario de quejas', 0),
('FORM_REPORTS',      'Reportes',           'Formulario de reportes', 0),
('FORM_CHANGE_PASS',  'Cambiar Contrasena', 'Formulario de cambio de contrasena', 1),
('FORM_PREFERENCES',  'Preferencias',       'Formulario de preferencias/idioma', 1);

-- Asignacion de permisos a roles

-- Admin (1): todos
INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
SELECT 1, id FROM [dbo].[permissions];

-- Profesor (2)
INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
SELECT 2, id FROM [dbo].[permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');

-- Alumno (3)
INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
SELECT 3, id FROM [dbo].[permissions] WHERE name IN
('FORM_COMPLAINTS');

-- Coordinador (4)
INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
SELECT 4, id FROM [dbo].[permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');

-- Usuarios (contrasena: 123 para todos)

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

-- Registrar migracion
INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
VALUES ('1.0.0', '02_v1.0.0_SeedData.sql', GETDATE());
