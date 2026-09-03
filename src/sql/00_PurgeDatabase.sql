use Trabajo_Final;

-- =============================================
-- PURGE: Eliminar todas las tablas (orden por FKs)
-- =============================================

IF OBJECT_ID('dbo.RoleHierarchy', 'U') IS NOT NULL DROP TABLE dbo.RoleHierarchy;
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL DROP TABLE dbo.Permissions;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;

-- Tablas viejas (por si existen)
IF OBJECT_ID('dbo.RoleMenuOptions', 'U') IS NOT NULL DROP TABLE dbo.RoleMenuOptions;
IF OBJECT_ID('dbo.MenuOptions', 'U') IS NOT NULL DROP TABLE dbo.MenuOptions;
