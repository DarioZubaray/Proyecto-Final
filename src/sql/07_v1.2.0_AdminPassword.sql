USE DarioZubaray_TF;

-- =============================================
-- MIGRACION v1.2.0: Usuario admin/admin
-- Cambia la contrasena del usuario admin al nuevo
-- hash BCrypt de "admin" y resetea su estado.
-- (El admin esta exento del bloqueo por 3 intentos
--  y requiere doble factor de autenticacion)
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.2.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- Hash BCrypt para password "admin": $2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G
        UPDATE [dbo].[users]
        SET [password_hash] = '$2a$11$h9OQXeWvXFC29T2cytB8HemgdlZQG35mnA6bmyZMTx3RNOMahmI0G',
            [is_active]     = 1,
            [retries_count] = 0,
            [last_update]   = GETDATE()
        WHERE [user_name] = 'admin';

        -- =============================================
        -- Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
        VALUES ('1.2.0', '07_v1.2.0_AdminPassword.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.2.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.2.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.2.0 ya fue aplicada anteriormente.';
END;
