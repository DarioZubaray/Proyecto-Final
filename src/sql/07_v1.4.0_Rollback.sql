USE [DarioZubaray_TF];
GO

-- =============================================
-- ROLLBACK v1.4.0
--
-- Revierte 06_v1.4.0_DVHGradesEvaluations.sql:
-- elimina las columnas [dvh] de [grades] y
-- [evaluations] conservando las tablas y sus
-- datos (a diferencia de 04_v1.3.0_Rollback.sql,
-- que elimina las tablas completas).
--
-- Los DROP van en lotes propios (SQL Server
-- compila cada lote contra el esquema previo).
--
-- Registra '1.4.0-rollback' y libera la version
-- '1.4.0' para poder re-ejecutar el incremental.
-- =============================================

-- =============================================
-- 1. Columna DVH en grades
-- =============================================
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.grades') AND name = 'dvh'
)
    ALTER TABLE [dbo].[grades] DROP COLUMN [dvh];
GO

-- =============================================
-- 2. Columna DVH en evaluations
-- =============================================
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.evaluations') AND name = 'dvh'
)
    ALTER TABLE [dbo].[evaluations] DROP COLUMN [dvh];
GO

-- =============================================
-- 3. Versiones: libera la 1.4.0 y registra el
--    rollback (el incremental puede re-ejecutarse)
-- =============================================
DELETE FROM [dbo].[schema_versions]
WHERE [version] = '1.4.0';

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[schema_versions]
    WHERE [version] = '1.4.0-rollback'
)
    INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
    VALUES ('1.4.0-rollback', '07_v1.4.0_Rollback.sql', GETDATE());
GO
