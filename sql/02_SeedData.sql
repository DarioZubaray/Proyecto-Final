use Trabajo_Final;

-- =============================================
-- Opciones de menú
-- =============================================
-- Opciones globales (visibles para todos los usuarios)
INSERT INTO [dbo].[MenuOptions] (name, label, description, is_global) VALUES
('DARK_MODE',        'Modo Oscuro',           'Cambiar tema claro/oscuro',          1),
('CHANGE_LANGUAGE',  'Cambiar Idioma',        'Seleccionar idioma de la interfaz',   1);

-- Opciones con permiso (visibles solo según rol)
INSERT INTO [dbo].[MenuOptions] (name, label, description, is_global) VALUES
('USERS_VIEW',       'ABM Usuarios',          'Ver listado de usuarios',             0),
('USERS_CREATE',     'Crear Usuarios',        'Crear nuevos usuarios',               0),
('USERS_EDIT',       'Editar Usuarios',       'Modificar datos de usuarios',         0),
('USERS_DELETE',     'Eliminar Usuarios',     'Eliminar usuarios del sistema',       0),
('ROLES_VIEW',       'ABM Roles',             'Ver y gestionar roles',               0),
('COMPLAINTS_VIEW',  'Ver Quejas',            'Consultar quejas recibidas',          0),
('COMPLAINTS_MANAGE', 'Gestionar Quejas',     'Responder y resolver quejas',         0),
('REPORTS_VIEW',     'Ver Reportes',          'Consultar reportes del sistema',      0),
('SETTINGS_EDIT',    'Configuración',         'Modificar configuración general',     0);

-- =============================================
-- Roles
-- =============================================
INSERT INTO [dbo].[Roles] (name) VALUES
('Admin'),
('Supervisor'),
('Operador');

-- =============================================
-- Asignación de opciones a roles
-- =============================================

-- Admin: ve todo
INSERT INTO [dbo].[RoleMenuOptions] (role_id, menu_option_id)
SELECT 1, id FROM [dbo].[MenuOptions] WHERE is_global = 0;

-- Supervisor: ve y edita usuarios, ve quejas y reportes
INSERT INTO [dbo].[RoleMenuOptions] (role_id, menu_option_id)
SELECT 2, id FROM [dbo].[MenuOptions] WHERE name IN
('USERS_VIEW', 'USERS_CREATE', 'USERS_EDIT', 'COMPLAINTS_VIEW', 'COMPLAINTS_MANAGE', 'REPORTS_VIEW');

-- Operador: solo ve usuarios y quejas
INSERT INTO [dbo].[RoleMenuOptions] (role_id, menu_option_id)
SELECT 3, id FROM [dbo].[MenuOptions] WHERE name IN
('USERS_VIEW', 'COMPLAINTS_VIEW');
