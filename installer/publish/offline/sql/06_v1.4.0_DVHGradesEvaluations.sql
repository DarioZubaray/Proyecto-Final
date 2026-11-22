USE [DarioZubaray_TF];
GO

-- =============================================
-- MIGRACION v1.4.0 (incremental): DVH en
-- evaluations y grades
--
-- Agrega la columna [dvh] (NVARCHAR(64)) a
-- [evaluations] y [grades] y calcula el backfill
-- SHA-256 con la misma serializacion canonica de
-- BLL.DVHHelperBLL: columnas menos [dvh], en orden
-- de columna, unidas por coma; fechas estilo 126
-- (sin fraccion si ms = 0); DATE convertido a
-- DATETIME para igualar el estilo 126 de la app;
-- SHA2_256 sobre NVARCHAR (bytes UTF-16LE, igual
-- que Encoding.Unicode).
--
-- Los ALTER van en lotes propios: SQL Server
-- compila cada lote completo contra el esquema
-- previo a su ejecucion, de modo que un UPDATE que
-- referencia una columna recien agregada en el
-- mismo lote fallaria con "Invalid column name".
--
-- Idempotente: puede re-ejecutarse. Los ALTER y el
-- backfill corren siempre (con sus guardas); solo
-- el registro de version se omite si ya existe.
--
-- Uso: 01 -> 03 -> 06       (instalacion / actualizacion)
--      01 -> 02 -> 03 -> 06 (demo, con datos de prueba)
--      05 consolidado en un paso (atajo dev, ya incluye
--      columnas + backfill + version 1.4.0)
-- Rollback: 07_v1.4.0_Rollback.sql
-- =============================================

-- =============================================
-- Lote 1: columna DVH en grades
-- =============================================
IF OBJECT_ID('dbo.grades', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.grades', 'dvh') IS NULL
    ALTER TABLE [dbo].[grades] ADD [dvh] NVARCHAR(64) NULL;
GO

-- =============================================
-- Lote 2: columna DVH en evaluations
-- =============================================
IF OBJECT_ID('dbo.evaluations', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.evaluations', 'dvh') IS NULL
    ALTER TABLE [dbo].[evaluations] ADD [dvh] NVARCHAR(64) NULL;
GO

-- =============================================
-- Lote 3: backfill del DVH + registro de version
-- =============================================
IF OBJECT_ID('dbo.evaluations', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.evaluations', 'dvh') IS NOT NULL
BEGIN
    UPDATE e
    SET e.[dvh] = LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256',
            CONCAT(
                CONVERT(NVARCHAR(MAX), e.[id]),                ',',
                CONVERT(NVARCHAR(MAX), e.[course_id]),         ',',
                e.[name],                                      ',',
                CONVERT(NVARCHAR(30), CAST(e.[date] AS DATETIME), 126), ',',
                CONVERT(NVARCHAR(MAX), e.[created_by]),        ',',
                CONVERT(NVARCHAR(30), e.[created_at], 126),    ',',
                CONVERT(NVARCHAR(30), e.[last_update], 126)
            )
        ), 2))
    FROM [dbo].[evaluations] e
    WHERE e.[dvh] IS NULL;
END

IF OBJECT_ID('dbo.grades', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.grades', 'dvh') IS NOT NULL
BEGIN
    UPDATE g
    SET g.[dvh] = LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256',
            CONCAT(
                CONVERT(NVARCHAR(MAX), g.[id]),             ',',
                CONVERT(NVARCHAR(MAX), g.[evaluation_id]),  ',',
                CONVERT(NVARCHAR(MAX), g.[student_id]),     ',',
                CONVERT(NVARCHAR(MAX), g.[grade])
            )
        ), 2))
    FROM [dbo].[grades] g
    WHERE g.[dvh] IS NULL;
END

-- Se omite si la version 1.4.0 ya fue registrada por este
-- script o por el consolidado (05).
IF NOT EXISTS (
    SELECT 1 FROM [dbo].[schema_versions]
    WHERE [version] = '1.4.0'
      AND [script_name] IN ('06_v1.4.0_DVHGradesEvaluations.sql', '05_v1.3.0_ScriptConsolidado.sql')
)
    INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
    VALUES ('1.4.0', '06_v1.4.0_DVHGradesEvaluations.sql', GETDATE());
GO
