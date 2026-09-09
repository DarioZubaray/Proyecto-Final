use Trabajo_Final;

-- =============================================
-- MIGRACION v1.1.0: Aulas y Cursos
-- Solo para bases que NO tienen estas tablas
-- (si se ejecutó 01_v1.0.0_CreateTables.sql, ya existen)
-- =============================================

-- 1. Verificar si esta migracion ya fue aplicada
IF NOT EXISTS (SELECT 1 FROM [dbo].[SchemaVersions] WHERE [Version] = '1.1.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 2. Tabla Aulas (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Aulas', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Aulas] (
                [Id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Nombre]      NVARCHAR(100)  NOT NULL,
                [Capacidad]   INT            NOT NULL,
                [Is_Active]   BIT            NOT NULL DEFAULT 1,
                [Created_At]  DATETIME       NOT NULL DEFAULT GETDATE(),
                [Last_Update] DATETIME       NOT NULL DEFAULT GETDATE()
            );
        END;

        -- =============================================
        -- 3. Tabla Cursos (solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.Cursos', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[Cursos] (
                [Id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Nombre]        NVARCHAR(200)  NOT NULL,
                [Descripcion]   NVARCHAR(500)  NULL,
                [Fecha_Inicio]  DATETIME       NOT NULL,
                [Fecha_Fin]     DATETIME       NOT NULL,
                [Aula_Id]       INT            NULL,
                [Is_Active]     BIT            NOT NULL DEFAULT 1,
                [Created_At]    DATETIME       NOT NULL DEFAULT GETDATE(),
                [Last_Update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursos_aulas FOREIGN KEY ([Aula_Id]) REFERENCES [dbo].[Aulas]([Id])
            );
        END;

        -- =============================================
        -- 4. Tabla CursoDocentes (N:N, solo si no existe)
        -- =============================================
        IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[CursoDocentes] (
                [Id]          INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [Curso_Id]    INT      NOT NULL,
                [Docente_Id]  INT      NOT NULL,
                [Is_Active]   BIT      NOT NULL DEFAULT 1,
                [Created_At]  DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_cursodocentes_cursos   FOREIGN KEY ([Curso_Id])   REFERENCES [dbo].[Cursos]([Id]),
                CONSTRAINT fk_cursodocentes_users    FOREIGN KEY ([Docente_Id]) REFERENCES [dbo].[Users]([Id])
            );
        END;

        -- =============================================
        -- 5. Permiso FORM_CURSO_MGMT
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [name] = 'FORM_CURSO_MGMT')
        BEGIN
            INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
            ('FORM_CURSO_MGMT', 'ABM Cursos', 'Formulario de gestion de cursos', 0);
        END;

        -- =============================================
        -- 6. Rol Coordinador
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [name] = 'Coordinador')
        BEGIN
            INSERT INTO [dbo].[Roles] (name) VALUES ('Coordinador');
        END;

        -- =============================================
        -- 7. Asignacion de permisos
        -- =============================================

        -- Admin (1): agregar permiso FORM_CURSO_MGMT
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        -- Coordinador (nuevo rol): todos los ABMs
        DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @coordinadorId, id FROM [dbo].[Permissions]
            WHERE name IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_CURSO_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS');
        END;

        -- Profesor (2): gestionar cursos
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[Permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        -- =============================================
        -- 8. Registrar migracion aplicada
        -- =============================================
        INSERT INTO [dbo].[SchemaVersions] (Version, ScriptName, AppliedAt)
        VALUES ('1.1.0', '04_v1.1.0_AulasCursos.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.1.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error al aplicar migracion v1.1.0: ' + ERROR_MESSAGE();
        THROW;
    END CATCH;
END
ELSE
BEGIN
    PRINT 'La migracion v1.1.0 ya fue aplicada anteriormente.';
END;
