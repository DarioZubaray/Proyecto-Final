use Trabajo_Final;

-- =============================================
-- Migración: Unificar modelo de permisos
-- Renombrar MenuOptions -> Permissions
-- Renombrar RoleMenuOptions -> RolePermissions
-- =============================================

-- 1. Crear tabla Permissions (nueva)
CREATE TABLE [dbo].[Permissions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name]        NVARCHAR(100)  NOT NULL UNIQUE,
    [label]       NVARCHAR(100)  NOT NULL,
    [description] NVARCHAR(256)  NULL
);

-- 2. Crear tabla RolePermissions (nueva)
CREATE TABLE [dbo].[RolePermissions] (
    [role_id]       INT NOT NULL,
    [permission_id] INT NOT NULL,
    PRIMARY KEY ([role_id], [permission_id]),
    CONSTRAINT fk_rolepermissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolepermissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[Permissions]([id])
);

-- 3. Migrar datos de MenuOptions -> Permissions
INSERT INTO [dbo].[Permissions] (name, label, description)
SELECT name, label, description FROM [dbo].[MenuOptions];

-- 4. Migrar datos de RoleMenuOptions -> RolePermissions
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT rmo.role_id, p.id
FROM [dbo].[RoleMenuOptions] rmo
INNER JOIN [dbo].[Permissions] p ON p.name = (
    SELECT name FROM [dbo].[MenuOptions] WHERE id = rmo.menu_option_id
);

-- 5. Eliminar tablas antiguas (en orden por FKs)
-- NOTA: Ejecutar solo después de verificar que la migración funcionó
-- ALTER TABLE [dbo].[RoleMenuOptions] DROP CONSTRAINT fk_rolemenuoptions_roles;
-- ALTER TABLE [dbo].[RoleMenuOptions] DROP CONSTRAINT fk_rolemenuoptions_menuoptions;
-- DROP TABLE [dbo].[RoleMenuOptions];
-- DROP TABLE [dbo].[MenuOptions];

-- 6. Seed data actualizado (si se crea desde cero)
-- Roles
INSERT INTO [dbo].[Roles] (name) VALUES ('Admin');

-- Permissions (antes MenuOptions)
INSERT INTO [dbo].[Permissions] (name, label, description) VALUES
('FORM_USER_MGMT',    'ABM Usuarios',          'Formulario de gestion de usuarios'),
('FORM_ROLE_MGMT',    'ABM Roles',             'Formulario de gestion de roles'),
('FORM_COMPLAINTS',   'Quejas',                'Formulario de quejas'),
('FORM_REPORTS',      'Reportes',              'Formulario de reportes'),
('FORM_SETTINGS',     'Configuracion',         'Formulario de configuracion'),
('FORM_CHANGE_PASS',  'Cambiar Contrasena',    'Formulario de cambio de contrasena'),
('FORM_PREFERENCES',  'Preferencias',          'Formulario de preferencias/idioma');

-- Admin: todos los permisos
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 1, id FROM [dbo].[Permissions];
