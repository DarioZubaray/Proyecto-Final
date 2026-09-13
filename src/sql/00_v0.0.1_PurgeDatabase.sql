use Trabajo_Final;

-- =============================================
-- PURGE v0.0.1: Eliminar todas las tablas
-- =============================================

-- Deshabilitar FK explícitamente en tablas que referencian Users y Roles
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql = @sql + N'ALTER TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N' NOCHECK CONSTRAINT ALL; '
FROM sys.foreign_keys fk
INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id;

EXEC sp_executesql @sql;

-- Eliminar todas las tablas
DECLARE @drop NVARCHAR(MAX) = N'';

SELECT @drop = @drop + N'DROP TABLE ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N'; '
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id;

EXEC sp_executesql @drop;

PRINT 'Purge completado exitosamente.';
