USE DarioZubaray_TF;

-- =============================================
-- PURGE v0.0.1: Eliminar todas las tablas
-- =============================================

-- 1. Deshabilitar FK de cada tabla
IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL
    ALTER TABLE dbo.attendance NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL
    ALTER TABLE dbo.course_students NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL
    ALTER TABLE dbo.course_teachers NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL
    ALTER TABLE dbo.courses NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL
    ALTER TABLE dbo.classrooms NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.activity_logs', 'U') IS NOT NULL
    ALTER TABLE dbo.activity_logs NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.role_hierarchy', 'U') IS NOT NULL
    ALTER TABLE dbo.role_hierarchy NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.role_permissions', 'U') IS NOT NULL
    ALTER TABLE dbo.role_permissions NOCHECK CONSTRAINT ALL;
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL
    ALTER TABLE dbo.users NOCHECK CONSTRAINT ALL;

-- 2. Eliminar todas las tablas
IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL DROP TABLE dbo.attendance;
IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL DROP TABLE dbo.course_students;
IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL DROP TABLE dbo.course_teachers;
IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL DROP TABLE dbo.courses;
IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL DROP TABLE dbo.classrooms;
IF OBJECT_ID('dbo.activity_logs', 'U') IS NOT NULL DROP TABLE dbo.activity_logs;
IF OBJECT_ID('dbo.role_hierarchy', 'U') IS NOT NULL DROP TABLE dbo.role_hierarchy;
IF OBJECT_ID('dbo.role_permissions', 'U') IS NOT NULL DROP TABLE dbo.role_permissions;
IF OBJECT_ID('dbo.schema_versions', 'U') IS NOT NULL DROP TABLE dbo.schema_versions;
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users;
IF OBJECT_ID('dbo.permissions', 'U') IS NOT NULL DROP TABLE dbo.permissions;
IF OBJECT_ID('dbo.roles', 'U') IS NOT NULL DROP TABLE dbo.roles;

PRINT 'Purge completado exitosamente.';
