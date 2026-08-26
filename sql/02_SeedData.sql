use Trabajo_Final;

-- =============================================
-- SEED: Datos iniciales
-- =============================================

-- =============================================
-- Roles
-- =============================================
INSERT INTO [dbo].[Roles] (name) VALUES
('Admin'),
('Profesor'),
('Alumno');

-- =============================================
-- Permisos (cada uno = 1 formulario)
-- =============================================
INSERT INTO [dbo].[Permissions] (name, label, description) VALUES
('FORM_USER_MGMT',    'ABM Usuarios',       'Formulario de gestion de usuarios'),
('FORM_ROLE_MGMT',    'ABM Roles',          'Formulario de gestion de roles'),
('FORM_COMPLAINTS',   'Quejas',             'Formulario de quejas'),
('FORM_REPORTS',      'Reportes',           'Formulario de reportes'),
('FORM_SETTINGS',     'Configuracion',      'Formulario de configuracion'),
('FORM_CHANGE_PASS',  'Cambiar Contrasena', 'Formulario de cambio de contrasena'),
('FORM_PREFERENCES',  'Preferencias',       'Formulario de preferencias/idioma');

-- =============================================
-- Asignación de permisos a roles
-- =============================================

-- Admin (1): todos los formularios
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 1, id FROM [dbo].[Permissions];

-- Profesor (2): gestiona usuarios, ve quejas y reportes
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 2, id FROM [dbo].[Permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');

-- Alumno (3): solo ve quejas
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 3, id FROM [dbo].[Permissions] WHERE name IN
('FORM_COMPLAINTS');

-- =============================================
-- Usuarios (contraseña: 123 para todos)
-- =============================================
INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language) VALUES
('admin',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es'),
('pepe',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 3, 'es'),
('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es');
