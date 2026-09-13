use Trabajo_Final;

-- =============================================
-- PURGE v0.0.1: Eliminar todas las tablas
-- Orden correcto respetando foreign keys
-- =============================================

-- 1. Tablas de Asistencia (v1.2.0)
IF OBJECT_ID('dbo.ClasesAlumnos', 'U') IS NOT NULL DROP TABLE dbo.ClasesAlumnos;

-- 2. Tablas de Inscripciones (v1.1.0)
IF OBJECT_ID('dbo.CursoAlumnos', 'U') IS NOT NULL DROP TABLE dbo.CursoAlumnos;

-- 3. Tablas de Cursos (v1.1.0)
IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NOT NULL DROP TABLE dbo.CursoDocentes;
IF OBJECT_ID('dbo.Cursos', 'U') IS NOT NULL DROP TABLE dbo.Cursos;
IF OBJECT_ID('dbo.Aulas', 'U') IS NOT NULL DROP TABLE dbo.Aulas;

-- 4. Tablas de Actividad
IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NOT NULL DROP TABLE dbo.ActivityLogs;

-- 5. Tablas de Jerarquía y Permisos
IF OBJECT_ID('dbo.RoleHierarchy', 'U') IS NOT NULL DROP TABLE dbo.RoleHierarchy;
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;

-- 6. Tablas de Versiones
IF OBJECT_ID('dbo.SchemaVersions', 'U') IS NOT NULL DROP TABLE dbo.SchemaVersions;

-- 7. Tablas Base (orden: primero las que tienen FK)
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL DROP TABLE dbo.Permissions;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;

-- 8. Tablas Legacy (por si existen de versiones anteriores)
IF OBJECT_ID('dbo.RoleMenuOptions', 'U') IS NOT NULL DROP TABLE dbo.RoleMenuOptions;
IF OBJECT_ID('dbo.MenuOptions', 'U') IS NOT NULL DROP TABLE dbo.MenuOptions;

PRINT 'Purge completado exitosamente.';
