use Trabajo_Final;

-- =============================================
-- ROLLBACK v1.1.0: Deshacer Aulas, Cursos,
-- Inscripciones, Asistencia y permisos asociados
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY

    -- =============================================
    -- 1. Eliminar tablas en orden correcto de FK
    -- =============================================
    IF OBJECT_ID('dbo.ClasesAlumnos', 'U') IS NOT NULL
        DROP TABLE dbo.ClasesAlumnos;

    IF OBJECT_ID('dbo.CursoAlumnos', 'U') IS NOT NULL
        DROP TABLE dbo.CursoAlumnos;

    IF OBJECT_ID('dbo.CursoDocentes', 'U') IS NOT NULL
        DROP TABLE dbo.CursoDocentes;

    IF OBJECT_ID('dbo.Cursos', 'U') IS NOT NULL
        DROP TABLE dbo.Cursos;

    IF OBJECT_ID('dbo.Aulas', 'U') IS NOT NULL
        DROP TABLE dbo.Aulas;

    -- =============================================
    -- 2. Eliminar permisos agregados en v1.1.0
    -- =============================================
    DELETE FROM [dbo].[RolePermissions]
    WHERE [permission_id] IN (
        SELECT id FROM [dbo].[Permissions]
        WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_MGMT', 'FORM_ASISTENCIA_VIEW')
    );

    DELETE FROM [dbo].[Permissions]
    WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_MGMT', 'FORM_ASISTENCIA_VIEW');

    -- =============================================
    -- 3. Restaurar permisos de Profesor (quitar lo que se quito en v1.3.0)
    -- =============================================
    DECLARE @profesorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Profesor');
    IF @profesorId IS NOT NULL
    BEGIN
        -- Restaurar FORM_USER_MGMT a Profesor
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @profesorId AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_USER_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @profesorId, id FROM [dbo].[Permissions] WHERE name = 'FORM_USER_MGMT';
        END;
    END;

    -- =============================================
    -- 4. Eliminar rol Coordinador
    -- =============================================
    DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
    IF @coordinadorId IS NOT NULL
    BEGIN
        DELETE FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId;

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
    -- 5. Registrar rollback
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
