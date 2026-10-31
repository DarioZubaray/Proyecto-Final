-- =============================================
-- MIGRACION v1.5.0: Alumnos como entidad + roles finales
--
-- 1. Crea la tabla [students]: los alumnos dejan de ser
--    usuarios (no inician sesion) y pasan a ser una entidad
--    gestionada, igual que cursos o aulas.
-- 2. Migra los usuarios con rol Alumno a [students]
--    conservando los mismos IDs (no se tocan los datos
--    historicos de course_students/attendance).
-- 3. Repunta las FK de course_students y attendance
--    de [users] a [students].
-- 4. Elimina los usuarios-alumno y el rol Alumno.
-- 5. Crea el rol Director (solo consulta de reportes).
-- 6. Agrega el permiso FORM_STUDENT_MGMT (ABM de alumnos).
-- 7. Ajusta permisos por rol:
--    - Profesor: sin FORM_CURSO_MGMT (cursos/aulas son
--      administrativos) ni FORM_INSCRIPCION_MGMT; queda como
--      operador de asistencia/evaluaciones/tickets/reportes.
--    - Coordinador: sin FORM_USER_MGMT ni FORM_ROLE_MGMT
--      (son de Admin); conserva el diario (cursos, aulas,
--      inscripciones, alumnos, tickets, reportes).
--    - Admin: todos los permisos incluido FORM_STUDENT_MGMT.
--    - Director: solo FORM_REPORTS.
-- =============================================

USE [DarioZubaray_TF];
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.5.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Tabla de alumnos (entidad gestionada)
        -- =============================================
        IF OBJECT_ID('[dbo].[students]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[students] (
                [id]            INT            IDENTITY(1,1) NOT NULL,
                [first_name]    NVARCHAR(100)  NOT NULL,
                [last_name]     NVARCHAR(100)  NOT NULL,
                [document]      NVARCHAR(20)   NOT NULL,
                [email]         NVARCHAR(150)  NULL,
                [phone]         NVARCHAR(50)   NULL,
                [is_active]     BIT            NOT NULL,
                [created_at]    DATETIME       NOT NULL,
                [last_update]   DATETIME       NOT NULL,
                CONSTRAINT [pk_students] PRIMARY KEY ([id]),
                CONSTRAINT [uq_students_document] UNIQUE ([document])
            );

            CREATE INDEX ix_students_name ON [dbo].[students] ([last_name], [first_name]);
        END;

        -- =============================================
        -- 2. Migracion de usuarios-alumno a students
        --    (mismos IDs: course_students/attendance no cambian)
        -- =============================================
        IF EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno')
        AND EXISTS (SELECT 1 FROM [dbo].[users] WHERE [role_id] = 3)
        BEGIN
            SET IDENTITY_INSERT [dbo].[students] ON;

            INSERT INTO [dbo].[students] ([id], [first_name], [last_name], [document], [email], [phone], [is_active], [created_at], [last_update])
            SELECT u.[id],
                   CASE WHEN CHARINDEX('_', u.[user_name]) > 0
                        THEN LEFT(u.[user_name], CHARINDEX('_', u.[user_name]) - 1)
                        ELSE u.[user_name] END,
                   CASE WHEN CHARINDEX('_', u.[user_name]) > 0
                        THEN SUBSTRING(u.[user_name], CHARINDEX('_', u.[user_name]) + 1, LEN(u.[user_name]))
                        ELSE u.[user_name] END,
                   'DNI-' + RIGHT('00000000' + CAST(u.[id] AS NVARCHAR(10)), 8),
                   NULL,
                   NULL,
                   1,
                   u.[created_at],
                   GETDATE()
            FROM [dbo].[users] u
            WHERE u.[role_id] = 3
              AND NOT EXISTS (SELECT 1 FROM [dbo].[students] s WHERE s.[id] = u.[id]);

            SET IDENTITY_INSERT [dbo].[students] OFF;

            -- ===========================================
            -- 3. Repuntar FKs de course_students/attendance
            -- ===========================================
            IF EXISTS (SELECT 1 FROM [dbo].sys.foreign_keys WHERE [name] = 'fk_course_students_users')
                ALTER TABLE [dbo].[course_students] DROP CONSTRAINT [fk_course_students_users];

            IF EXISTS (SELECT 1 FROM [dbo].sys.foreign_keys WHERE [name] = 'fk_attendance_users')
                ALTER TABLE [dbo].[attendance] DROP CONSTRAINT [fk_attendance_users];

            IF NOT EXISTS (SELECT 1 FROM [dbo].sys.foreign_keys WHERE [name] = 'fk_course_students_students')
                ALTER TABLE [dbo].[course_students] WITH CHECK
                    ADD CONSTRAINT [fk_course_students_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]);

            IF NOT EXISTS (SELECT 1 FROM [dbo].sys.foreign_keys WHERE [name] = 'fk_attendance_students')
                ALTER TABLE [dbo].[attendance] WITH CHECK
                    ADD CONSTRAINT [fk_attendance_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]);

            -- ===========================================
            -- 4. Eliminar usuarios-alumno y rol Alumno
            --    (dependencias FK primero: tickets y bitacora)
            -- ===========================================
            DELETE t
            FROM [dbo].[tickets] t
            INNER JOIN [dbo].[users] u ON u.[id] = t.[user_id]
            WHERE u.[role_id] = 3;

            DELETE a
            FROM [dbo].[activity_logs] a
            INNER JOIN [dbo].[users] u ON u.[id] = a.[user_id]
            WHERE u.[role_id] = 3;

            DELETE FROM [dbo].[users] WHERE [role_id] = 3;

            DELETE FROM [dbo].[role_permissions] WHERE [role_id] = 3;
            DELETE FROM [dbo].[roles] WHERE [id] = 3 AND [name] = 'Alumno';
        END;

        -- =============================================
        -- 5. Rol Director (solo consulta de reportes)
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Director')
        BEGIN
            INSERT INTO [dbo].[roles] ([name]) VALUES ('Director');
        END;

        -- =============================================
        -- 6. Permiso ABM de alumnos
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_STUDENT_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_STUDENT_MGMT', 'ABM Alumnos', 'Gestion de alumnos (entidad, no usuarios)', 0);
        END;

        -- =============================================
        -- 7. Ajuste de permisos por rol
        -- =============================================

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2): operador especial; sin ABM de cursos/aulas ni inscripciones
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 2
          AND p.[name] IN ('FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT');

        -- Coordinador (4): tareas diarias; sin usuarios/roles (son de Admin)
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] = 4
          AND p.[name] IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT');

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 4 AND p.[name] = 'FORM_STUDENT_MGMT'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 4, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_STUDENT_MGMT';
        END;

        -- Director (5): solo consulta de reportes
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 5 AND p.[name] = 'FORM_REPORTS'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 5, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_REPORTS';
        END;

        -- =============================================
        -- Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
        VALUES ('1.5.0', '11_v1.5.0_EstudiantesYRoles.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.5.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        PRINT 'ERROR en la migracion v1.5.0: ' + @ErrorMessage;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
ELSE
BEGIN
    PRINT 'La migracion v1.5.0 ya fue aplicada.';
END
GO
