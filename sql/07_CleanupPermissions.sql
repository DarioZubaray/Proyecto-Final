use Trabajo_Final;

-- =============================================
-- LIMPIEZA: Eliminar permisos viejos y asignar FORM_*
-- =============================================

-- 1. Borrar relaciones viejas de RolePermissions
DELETE FROM [dbo].[RolePermissions];

-- 2. Borrar permisos viejos (los que NO son FORM_*)
DELETE FROM [dbo].[Permissions] WHERE name NOT LIKE 'FORM_%';

-- 3. Asignar permisos FORM_* a roles

-- Admin (id=1): todos los FORM_*
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 1, id FROM [dbo].[Permissions];

-- Profesor (id=2): solo formularios de lectura/gestion
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 2, id FROM [dbo].[Permissions] WHERE name IN
('FORM_USER_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');

-- Alumno (id=3): solo ver quejas (nada de ABM)
INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
SELECT 3, id FROM [dbo].[Permissions] WHERE name IN
('FORM_COMPLAINTS');

-- 4. Verificar resultado
SELECT r.name AS Rol, p.name AS Permiso, p.label AS Formulario
FROM RolePermissions rp
INNER JOIN Roles r ON r.id = rp.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
ORDER BY r.name, p.name;
