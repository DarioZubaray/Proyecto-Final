USE [DarioZubaray_TF];
GO

-- =============================================
-- ROLLBACK v1.3.0
--
-- Revierte el script incremental
-- (03_v1.3.0_Incremental.sql) y devuelve la base al
-- estado v1 (01_v1.0.0_CreateTables):
--
--   1. Elimina las tablas agregadas: grades,
--      evaluations, tickets, attendance,
--      course_students, course_teachers, courses,
--      classrooms y students.
--   2. Elimina la columna users.dvh (DVH).
--   3. Elimina los permisos agregados por el
--      incremental (10) y restaura la matriz de
--      permisos del seed.
--   4. Elimina el rol Director (sus usuarios pasan
--      a Admin) y recrea el rol Alumno (id 3).
--   5. Restaura los usuarios-alumno del seed.
--   6. Registra '1.3.0-rollback' y libera la
--      version '1.3.0' para poder re-ejecutar el
--      incremental.
--
-- Nota: el rollback es destructivo (borra tablas y
-- datos academicos) y deja la base incompatible con
-- la aplicacion actual: para volver a usarla hay que
-- re-ejecutar 03_v1.3.0_Incremental.sql.
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY

    -- =============================================
    -- 1. Tablas agregadas por el incremental
    --    (orden inverso: primero las hijas)
    -- =============================================
    IF OBJECT_ID('dbo.grades', 'U') IS NOT NULL
        DROP TABLE [dbo].[grades];

    IF OBJECT_ID('dbo.evaluations', 'U') IS NOT NULL
        DROP TABLE [dbo].[evaluations];

    IF OBJECT_ID('dbo.tickets', 'U') IS NOT NULL
        DROP TABLE [dbo].[tickets];

    IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL
        DROP TABLE [dbo].[attendance];

    IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL
        DROP TABLE [dbo].[course_students];

    IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL
        DROP TABLE [dbo].[course_teachers];

    IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL
        DROP TABLE [dbo].[courses];

    IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL
        DROP TABLE [dbo].[classrooms];

    IF OBJECT_ID('dbo.students', 'U') IS NOT NULL
        DROP TABLE [dbo].[students];

    -- =============================================
    -- 2. Columna DVH
    -- =============================================
    IF EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.users') AND name = 'dvh'
    )
        ALTER TABLE [dbo].[users] DROP COLUMN [dvh];

    -- =============================================
    -- 3. Permisos: primero las asignaciones (wipe
    --    completo) y luego los permisos agregados.
    --    Quedan solo los 5 permisos del seed v1
    --    (sin FORM_COMPLAINTS: dado de baja por la
    --    alineacion del incremental).
    -- =============================================
    DELETE FROM [dbo].[role_permissions];

    DELETE FROM [dbo].[permissions]
    WHERE [name] IN (
        'FORM_CURSO_MGMT',
        'FORM_INSCRIPCION_MGMT',
        'FORM_ASISTENCIA_MGMT',
        'FORM_ASISTENCIA_VIEW',
        'FORM_TICKET_MGMT',
        'FORM_TICKET_CREATE',
        'FORM_STUDENT_MGMT',
        'FORM_EVAL_MGMT',
        'FORM_REPORT_DETAIL',
        'FORM_BACKUP_MGMT',
        'FORM_ACTIVITY_HISTORY',
        'FORM_LOGOUT',
        'FORM_ABOUT',
        'FORM_CLASSROOM_MGMT'
    );

    UPDATE [dbo].[permissions]
    SET [is_system] = 0
    WHERE [name] IN ('FORM_CHANGE_PASS', 'FORM_PREFERENCES');

    -- =============================================
    -- 4. Rol Director: sus usuarios pasan a Admin
    --    y el rol se elimina.
    -- =============================================
    UPDATE [dbo].[users]
    SET [role_id] = 1
    WHERE [role_id] = (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Director');

    DELETE FROM [dbo].[roles]
    WHERE [name] = 'Director';

    -- =============================================
    -- 5. Rol Alumno: se recrea con el id 3 del seed
    --    (el incremental lo exige con ese id).
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Alumno')
    BEGIN
        SET IDENTITY_INSERT [dbo].[roles] ON;

        INSERT INTO [dbo].[roles] ([id], [name])
        VALUES (3, 'Alumno');

        SET IDENTITY_INSERT [dbo].[roles] OFF;
    END;

    -- =============================================
    -- 6. Matriz de permisos exacta del seed v1.0.0
    -- =============================================
    DECLARE @adminId       INT = (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Admin');
    DECLARE @profesorId    INT = (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Profesor');
    DECLARE @alumnoId      INT = (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Alumno');
    DECLARE @coordinadorId INT = (SELECT [id] FROM [dbo].[roles] WHERE [name] = 'Coordinador');

    -- Admin (1): todos los permisos
    IF @adminId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
        SELECT @adminId, id FROM [dbo].[permissions];
    END;

    -- Profesor (2)
    IF @profesorId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
        SELECT @profesorId, id FROM [dbo].[permissions]
        WHERE name IN ('FORM_CHANGE_PASS', 'FORM_PREFERENCES');
    END;

    -- Alumno (3)
    IF @alumnoId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
        SELECT @alumnoId, id FROM [dbo].[permissions]
        WHERE name IN ('FORM_CHANGE_PASS', 'FORM_PREFERENCES');
    END;

    -- Coordinador (4)
    IF @coordinadorId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
        SELECT @coordinadorId, id FROM [dbo].[permissions]
        WHERE name IN ('FORM_CHANGE_PASS', 'FORM_PREFERENCES');
    END;

    -- =============================================
    -- 7. Usuarios-alumno del seed (15). Se insertan
    --    solo si faltan; conservan el rol Alumno y
    --    el login desactivado (is_active = 0).
    --    Hash BCrypt "123":
    --    $2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy
    -- =============================================
    IF @alumnoId IS NOT NULL
    BEGIN
        INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme)
        SELECT v.user_name,
               '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',
               0, 0, GETDATE(), GETDATE(), @alumnoId, 'es', 'System'
        FROM (VALUES
            ('alumno_perez'),
            ('alumno_gomez'),
            ('alumno_diaz'),
            ('alumno_morales'),
            ('alumno_torres'),
            ('alumno_ramos'),
            ('alumno_silva'),
            ('alumno_castro'),
            ('alumno_ruiz'),
            ('alumno_alvarez'),
            ('alumno_romero'),
            ('alumno_sanchez'),
            ('alumno_herrera'),
            ('alumno_patricio'),
            ('alumno_soto')
        ) AS v(user_name)
        WHERE NOT EXISTS (SELECT 1 FROM [dbo].[users] u WHERE u.[user_name] = v.user_name);
    END;

    -- =============================================
    -- 8. Versiones: libera la 1.3.0 y registra el
    --    rollback (el incremental puede re-ejecutarse)
    -- =============================================
    DELETE FROM [dbo].[schema_versions]
    WHERE [version] = '1.3.0';

    INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
    VALUES ('1.3.0-rollback', '04_v1.3.0_Rollback.sql', GETDATE());

    COMMIT TRANSACTION;

    PRINT 'Rollback v1.3.0 completado exitosamente. Base restaurada al estado v1.';

END TRY
BEGIN CATCH

    ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();

    PRINT 'ERROR en el rollback v1.3.0: ' + @ErrorMessage;

    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

END CATCH
GO
