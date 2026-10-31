-- =============================================
-- MIGRACION v1.6.0: Evaluaciones y Notas (CU-10)
--
-- 1. Crea la tabla [evaluations]: una evaluacion
--    pertenece a un curso y fue creada por un usuario
--    (el docente que la dicta).
-- 2. Crea la tabla [grades]: la nota de un alumno
--    en una evaluacion (0 a 10), unica por alumno y
--    evaluacion.
-- 3. Agrega el permiso FORM_EVAL_MGMT (menú
--    Académico -> Evaluaciones).
-- 4. Ajusta permisos por rol:
--    - Profesor: otorga FORM_EVAL_MGMT (opera sus
--      cursos).
--    - Admin: todos los permisos (incluye el nuevo).
--    - Coordinador y Director: sin FORM_EVAL_MGMT
--      (solo Profesor carga notas).
-- =============================================

USE [DarioZubaray_TF];
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.6.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 1. Tabla de evaluaciones
        -- =============================================
        IF OBJECT_ID('[dbo].[evaluations]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[evaluations] (
                [id]           INT            IDENTITY(1,1) NOT NULL,
                [course_id]    INT            NOT NULL,
                [name]         NVARCHAR(200)  NOT NULL,
                [date]         DATE           NOT NULL,
                [created_by]   INT            NOT NULL,
                [created_at]   DATETIME       NOT NULL,
                [last_update]  DATETIME       NOT NULL,
                CONSTRAINT [pk_evaluations] PRIMARY KEY ([id]),
                CONSTRAINT [fk_evaluations_courses] FOREIGN KEY ([course_id]) REFERENCES [dbo].[courses]([id]),
                CONSTRAINT [fk_evaluations_users] FOREIGN KEY ([created_by]) REFERENCES [dbo].[users]([id])
            );

            CREATE INDEX ix_evaluations_course ON [dbo].[evaluations] ([course_id], [date]);
        END;

        -- =============================================
        -- 2. Tabla de notas
        -- =============================================
        IF OBJECT_ID('[dbo].[grades]', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[grades] (
                [id]             INT           IDENTITY(1,1) NOT NULL,
                [evaluation_id]  INT           NOT NULL,
                [student_id]     INT           NOT NULL,
                [grade]          DECIMAL(4,2)  NOT NULL,
                CONSTRAINT [pk_grades] PRIMARY KEY ([id]),
                CONSTRAINT [fk_grades_evaluations] FOREIGN KEY ([evaluation_id]) REFERENCES [dbo].[evaluations]([id]),
                CONSTRAINT [fk_grades_students] FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]),
                CONSTRAINT [uq_grades_evaluation_student] UNIQUE ([evaluation_id], [student_id]),
                CONSTRAINT [ck_grades_range] CHECK ([grade] >= 0 AND [grade] <= 10)
            );
        END;

        -- =============================================
        -- 3. Permiso de evaluaciones
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_EVAL_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] ([name], [label], [description], [is_system])
            VALUES ('FORM_EVAL_MGMT', 'Evaluaciones', 'Registro de evaluaciones y notas de los cursos del docente', 0);
        END;

        -- =============================================
        -- 4. Ajuste de permisos por rol
        -- =============================================

        -- Admin (1): todos los permisos (incluye el nuevo)
        INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
        SELECT 1, p.[id]
        FROM [dbo].[permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            WHERE rp.[role_id] = 1 AND rp.[permission_id] = p.[id]
        );

        -- Profesor (2): otorga FORM_EVAL_MGMT si falta
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[role_permissions] rp
            INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
            WHERE rp.[role_id] = 2 AND p.[name] = 'FORM_EVAL_MGMT'
        )
        BEGIN
            INSERT INTO [dbo].[role_permissions] ([role_id], [permission_id])
            SELECT 2, p.[id] FROM [dbo].[permissions] p WHERE p.[name] = 'FORM_EVAL_MGMT';
        END;

        -- Coordinador (4) y Director (5): sin FORM_EVAL_MGMT
        DELETE rp
        FROM [dbo].[role_permissions] rp
        INNER JOIN [dbo].[permissions] p ON p.[id] = rp.[permission_id]
        WHERE rp.[role_id] IN (4, 5)
          AND p.[name] = 'FORM_EVAL_MGMT';

        -- =============================================
        -- Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] ([version], [script_name], [applied_at])
        VALUES ('1.6.0', '12_v1.6.0_Evaluaciones.sql', GETDATE());

        COMMIT TRANSACTION;

        PRINT 'Migracion v1.6.0 aplicada exitosamente.';

    END TRY
    BEGIN CATCH

        ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        PRINT 'ERROR en la migracion v1.6.0: ' + @ErrorMessage;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
ELSE
BEGIN
    PRINT 'La migracion v1.6.0 ya fue aplicada.';
END
GO
