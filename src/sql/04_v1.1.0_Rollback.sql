use Trabajo_Final;

-- =============================================
-- ROLLBACK v1.1.0
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY

    IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL
        DROP TABLE dbo.attendance;

    IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL
        DROP TABLE dbo.course_students;

    IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL
        DROP TABLE dbo.course_teachers;

    IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL
        DROP TABLE dbo.courses;

    IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL
        DROP TABLE dbo.classrooms;

    DELETE FROM [dbo].[RolePermissions]
    WHERE [permission_id] IN (
        SELECT id FROM [dbo].[Permissions]
        WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_MGMT', 'FORM_ASISTENCIA_VIEW')
    );

    DELETE FROM [dbo].[Permissions]
    WHERE name IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_ASISTENCIA_MGMT', 'FORM_ASISTENCIA_VIEW');

    DECLARE @profesorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Profesor');
    IF @profesorId IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [dbo].[RolePermissions] WHERE [role_id] = @profesorId AND [permission_id] = (SELECT id FROM [dbo].[Permissions] WHERE name = 'FORM_USER_MGMT'))
        BEGIN
            INSERT INTO [dbo].[RolePermissions] (role_id, permission_id)
            SELECT @profesorId, id FROM [dbo].[Permissions] WHERE name = 'FORM_USER_MGMT';
        END;
    END;

    DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[Roles] WHERE name = 'Coordinador');
    IF @coordinadorId IS NOT NULL
    BEGIN
        DELETE FROM [dbo].[RolePermissions] WHERE [role_id] = @coordinadorId;
        IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [role_id] = @coordinadorId)
        BEGIN
            DELETE FROM [dbo].[Roles] WHERE [id] = @coordinadorId;
        END;
    END;

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
