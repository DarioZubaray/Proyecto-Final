USE DarioZubaray_TF;

-- =============================================
-- MIGRACION v1.3.0: Digito Verificador Horizontal (DVH)
-- Agrega la columna [dvh] a [users] y calcula el hash
-- SHA-256 de cada fila (todas las columnas menos [dvh],
-- en orden de columna, unidas por coma, sin espacios).
--
-- El backfill replica exactamente la serializacion
-- canonica que genera la aplicacion (BLL.DVHHelperBLL):
--   id, user_name, password_hash, is_active (1/0),
--   retries_count, last_update, created_at, role_id,
--   language, theme
--   - fechas: estilo 126 -> yyyy-MM-ddTHH:mm:ss.fff
--   - role_id NULL -> cadena vacia
--   - NULL -> cadena vacia (CONCAT trata NULL como '')
--   - hash: SHA2_256 en hex, minusculas. HASHBYTES hashea el
--     argumento NVARCHAR como bytes UTF-16LE, igual que
--     Encoding.Unicode en BLL.DVHHelperBLL (no UTF-8).
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.3.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Columna DVH
        -- =============================================
        IF NOT EXISTS (
            SELECT 1 FROM sys.columns
            WHERE object_id = OBJECT_ID('dbo.users') AND name = 'dvh'
        )
        BEGIN
            ALTER TABLE [dbo].[users] ADD [dvh] NVARCHAR(64) NULL;
        END;

        -- =============================================
        -- 2. Backfill de todas las filas existentes
        -- =============================================
        UPDATE u
        SET u.[dvh] = LOWER(CONVERT(VARCHAR(64),
            HASHBYTES('SHA2_256',
                CONCAT(
                    CONVERT(NVARCHAR(MAX), u.[id]),          ',',
                    u.[user_name],                           ',',
                    u.[password_hash],                       ',',
                    CASE WHEN u.[is_active] = 1 THEN '1' ELSE '0' END, ',',
                    CONVERT(NVARCHAR(MAX), u.[retries_count]), ',',
                    CONVERT(NVARCHAR(30), u.[last_update], 126), ',',
                    CONVERT(NVARCHAR(30), u.[created_at], 126),  ',',
                    CASE WHEN u.[role_id] IS NULL THEN '' ELSE CONVERT(NVARCHAR(MAX), u.[role_id]) END, ',',
                    u.[language],                            ',',
                    u.[theme]
                )
            ), 2))
        FROM [dbo].[users] u
        WHERE u.[dvh] IS NULL;

        -- =============================================
        -- 3. Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
        VALUES ('1.3.0', '08_v1.3.0_DVH.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.3.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.3.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.3.0 ya fue aplicada anteriormente.';
END;
