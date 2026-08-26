use Trabajo_Final;

-- =============================================
-- QUERIES: Consultas útiles para desarrollo
-- =============================================

-- Ver todos los usuarios con su rol
SELECT u.id, u.user_name, r.name AS rol, u.language, u.is_active
FROM Users u
LEFT JOIN Roles r ON r.id = u.role_id;

-- Ver permisos de cada rol
SELECT r.name AS rol, p.name AS permiso, p.label AS formulario
FROM RolePermissions rp
INNER JOIN Roles r ON r.id = rp.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
ORDER BY r.name, p.name;

-- Ver jerarquía de roles
SELECT r1.name AS padre, r2.name AS hijo
FROM RoleHierarchy rh
INNER JOIN Roles r1 ON r1.id = rh.parent_role_id
INNER JOIN Roles r2 ON r2.id = rh.child_role_id;

-- Verificar si un usuario tiene permiso específico
-- (cambiar 'pepe' y 'FORM_USER_MGMT' según necesidad)
SELECT u.user_name, p.name AS permiso
FROM Users u
INNER JOIN RolePermissions rp ON rp.role_id = u.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
WHERE u.user_name = 'pepe' AND p.name = 'FORM_USER_MGMT';

-- Contar usuarios por rol
SELECT r.name AS rol, COUNT(u.id) AS cantidad
FROM Roles r
LEFT JOIN Users u ON u.role_id = r.id
GROUP BY r.name;
