USE DarioZubaray_TF;

-- =============================================
-- MIGRACION v1.4.0: Tickets / Reclamos
-- Crea la tabla [tickets] (reportes de usuarios)
-- y el permiso FORM_TICKET_MGMT para la gestion
-- administrativa (listar todos y cerrar).
--
-- Cualquier usuario autenticado puede crear tickets;
-- solo el rol Admin (1) gestiona y cierra.
-- Estados: Abierto (is_closed = 0) / Cerrado (is_closed = 1).
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.4.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Tabla tickets
        -- =============================================
        IF OBJECT_ID('dbo.tickets', 'U') IS NOT NULL
        BEGIN
            -- Tabla creada por una instalacion previa distinta: se recrea limpia.
            DROP TABLE [dbo].[tickets];
        END;

        CREATE TABLE [dbo].[tickets] (
            [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
            [user_id]     INT            NOT NULL,
            [subject]     NVARCHAR(200)  NOT NULL,
            [description] NVARCHAR(MAX)  NOT NULL,
            [is_closed]   BIT            NOT NULL DEFAULT 0,
            [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
            [closed_at]   DATETIME       NULL,
            [closed_by]   INT            NULL,
            CONSTRAINT fk_tickets_user FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]),
            CONSTRAINT fk_tickets_closed_by FOREIGN KEY ([closed_by]) REFERENCES [dbo].[users]([id])
        );
        CREATE INDEX ix_tickets_user_created ON [dbo].[tickets] ([user_id], [created_at] DESC);
        CREATE INDEX ix_tickets_is_closed ON [dbo].[tickets] ([is_closed], [created_at] DESC);

        -- =============================================
        -- 2. Permiso de gestion administrativa
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_TICKET_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system)
            VALUES ('FORM_TICKET_MGMT', 'Gestion de Tickets', 'Listar y cerrar tickets de todos los usuarios', 0);
        END;

        -- Asignacion al rol Admin (1); los demas roles crean tickets sin necesitar permiso.
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions]
            WHERE [role_id] = 1
              AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT')
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_TICKET_MGMT';
        END;

        -- =============================================
        -- 3. Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
        VALUES ('1.4.0', '09_v1.4.0_Tickets.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.4.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.4.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.4.0 ya fue aplicada anteriormente.';
END;
