USE DarioZubaray_TF;

-- =============================================
-- MIGRACION v1.4.1: Ajustes de actores
-- 1. Alumnos pasan a ser solo datos del sistema
--    (inscripcion/asistencia): se desactiva su
--    login (is_active = 0).
-- 2. Tickets: el rol Coordinador obtiene
--    FORM_TICKET_MGMT (listar todos y cerrar);
--    Profesor crea tickets sin gestionar.
--    Admin (oculto) puede ambas cosas.
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.4.1')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Desactivar login de alumnos
        -- =============================================
        UPDATE u
        SET u.[is_active] = 0
        FROM [dbo].[users] u
        INNER JOIN [dbo].[roles] r ON r.[id] = u.[role_id]
        WHERE r.[name] = 'Alumno'
          AND u.[is_active] = 1;

        -- =============================================
        -- 2. Permiso de gestion de tickets al Coordinador
        -- =============================================
        IF EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_TICKET_MGMT')
        AND EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Coordinador')
        AND NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions]
            WHERE [role_id] = (SELECT id FROM [dbo].[roles] WHERE name = 'Coordinador')
              AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT')
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT r.[id], p.[id]
            FROM [dbo].[roles] r
            CROSS JOIN [dbo].[permissions] p
            WHERE r.[name] = 'Coordinador'
              AND p.[name] = 'FORM_TICKET_MGMT';
        END;

        -- =============================================
        -- 3. Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
        VALUES ('1.4.1', '10_v1.4.1_AjusteActores.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.4.1 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.4.1: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.4.1 ya fue aplicada anteriormente.';
END;
