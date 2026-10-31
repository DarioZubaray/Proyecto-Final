-- =============================================
-- MIGRACION v1.8.0: Respaldo y Restauracion (CU-13)
--
-- 1. Crea el permiso FORM_BACKUP_MGMT (menu
--    Administracion -> Respaldo y Restauracion).
-- 2. Ajusta permisos por rol:
--    - Admin: todos los permisos (incluye el nuevo).
--    - Profesor, Coordinador y Director: sin
--      FORM_BACKUP_MGMT (operacion exclusiva de
--      Admin).
-- =============================================

USE [DarioZubaray_TF];
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.8.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Permiso de respaldo
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_BACKUP_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_BACKUP_MGMT', 'Respaldo y Restauracion', 'Generacion de respaldos y restauracion de la base de datos', 0);
        END;

        -- =============================================
        -- 2. Ajuste de permisos por rol
        -- =============================================

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2), Coordinador (4) y Director (5): sin FORM_BACKUP_MGMT
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] IN (2, 4, 5)
          AND p.[name] = 'FORM_BACKUP_MGMT';

        -- =============================================
        -- Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
        VALUES ('1.8.0', '14_v1.8.0_Respaldo.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.8.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        PRINT 'ERROR en la migracion v1.8.0: ' + @ErrorMessage;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
ELSE
BEGIN
    PRINT 'La migracion v1.8.0 ya fue aplicada.';
END
GO
