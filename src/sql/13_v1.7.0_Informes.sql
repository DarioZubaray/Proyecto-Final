-- =============================================
-- MIGRACION v1.7.0: Reportes (CU-11 y CU-12)
--
-- 1. Crea el permiso FORM_REPORT_DETAIL (menú
--    Reportes -> Informes Personalizados, CU-12).
-- 2. Ajusta permisos por rol:
--    - Admin: todos los permisos (incluye el nuevo).
--    - Coordinador y Director: FORM_REPORTS (CU-11)
--      y FORM_REPORT_DETAIL (CU-12).
--    - Profesor: FORM_REPORTS (CU-11, solo sus
--      cursos) sin FORM_REPORT_DETAIL.
-- FORM_REPORTS ya existe desde v1.0.0 (seed).
-- =============================================

USE [DarioZubaray_TF];
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.7.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Permiso de informes personalizados (CU-12)
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_REPORT_DETAIL')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_REPORT_DETAIL', 'Informes Personalizados', 'Generacion y exportacion de informes academicos por curso', 0);
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

        -- Coordinador (4): FORM_REPORTS (CU-11) y FORM_REPORT_DETAIL (CU-12)
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 4 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 4, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 4 AND p.[name] = 'FORM_REPORT_DETAIL'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 4, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORT_DETAIL';
        END;

        -- Director (5): solo consulta de reportes (CU-11 y CU-12)
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORT_DETAIL'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORT_DETAIL';
        END;

        -- Profesor (2): FORM_REPORTS (CU-11, solo sus cursos)
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 2 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 2, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        -- Profesor (2): sin FORM_REPORT_DETAIL (informes solo Coordinador y Director)
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 2
          AND p.[name] = 'FORM_REPORT_DETAIL';

        -- =============================================
        -- Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
        VALUES ('1.7.0', '13_v1.7.0_Informes.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.7.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        PRINT 'ERROR en la migracion v1.7.0: ' + @ErrorMessage;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
ELSE
BEGIN
    PRINT 'La migracion v1.7.0 ya fue aplicada.';
END
GO
