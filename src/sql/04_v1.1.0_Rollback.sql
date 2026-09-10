use Trabajo_Final;

-- =============================================
-- ROLLBACK v1.1.0: Deshacer Aulas, Cursos e Inscripciones
-- Elimina tablas, permisos y rol Coordinador
-- Funciona aunque la migracion no se haya registrado
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY

    -- =============================================
    -- 1. Eliminar tablas en orden correcto de FK
    -- =============================================
    IF OBJECT_ID('dbo.CursoAlumnos', 'U') IS NOT NULL
        DROP TABLE dbo.CursoAlumnos;

    IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NOT NULL
        DROP TABLE dbo.CursoDocentes;

    IF OBJECT_ID('dbo.Cursos', 'U') IS NOT NULL
        DROP TABLE dbo.Cursos;

    IF OBJECT_ID('dbo.Aulas', 'U') IS NOT NULL
        DROP TABLE dbo.Aulas;

    -- =============================================
    -- 2. Eliminar permisos de inscripciones
    -- =============================================
    DELETE FROM [dbo].[RolePermissions]
    WHERE [permission_id] IN (
        SELECT id FROM [dbo].[Permissions]
        WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT')
    );

    DELETE FROM [dbo].[Permissions]
    WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT');

    -- =============================================
    -- 3. Eliminar permisos de Coordinador
    -- =============================================
    DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
    IF @coordinadorId IS NOT NULL
    BEGIN
        -- Quitar permisos del Coordinador
        DELETE FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId;

        -- Solo borrar el rol si no hay usuarios asignados
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [role_id] = @coordinadorId)
        BEGIN
            DELETE FROM [dbo].[Roles] WHERE [id] = @coordinadorId;
        END
        ELSE
        BEGIN
            PRINT 'No se pudo eliminar el rol Coordinador porque tiene usuarios asignados.';
        END;
    END;

    -- =============================================
    -- 4. Registrar rollback (si existe la tabla)
    -- =============================================
    IF OBJECT_ID('dbo.SchemaVersions', 'U') IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[SchemaVersions] (Version, ScriptName, AppliedAt)
        VALUES ('1.1.0-rollback', '04_v1.1.0_Rollback.sql', GETDATE());
    END;

    COMMIT TRANSACTION;

    PRINT 'Rollback v1.1.0 completado exitosamente.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error al ejecutar rollback v1.1.0: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
