use Trabajo_Final;

-- =============================================
-- MIGRACION v1.1.0: Classrooms, Courses,
-- Course Teachers, Course Students, Attendance
-- Tables + Seed data
-- =============================================

IF NOT EXISTS (SELECT 1 FROM [dbo].[schema_versions] WHERE [version] = '1.1.0')
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        -- =============================================
        -- 2. Table: classrooms
        -- =============================================
        IF OBJECT_ID('dbo.classrooms', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[classrooms] (
                [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [name]         NVARCHAR(100)  NOT NULL,
                [capacity]     INT            NOT NULL,
                [is_active]    BIT            NOT NULL DEFAULT 1,
                [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
            );
        END;

        -- =============================================
        -- 3. Table: courses
        -- =============================================
        IF OBJECT_ID('dbo.courses', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[courses] (
                [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                [name]          NVARCHAR(200)  NOT NULL,
                [description]   NVARCHAR(500)  NULL,
                [start_date]    DATETIME       NOT NULL,
                [end_date]      DATETIME       NOT NULL,
                [classroom_id]  INT            NOT NULL,
                [day_of_week]   INT            NULL,
                [start_time]    TIME           NULL,
                [end_time]      TIME           NULL,
                [is_active]     BIT            NOT NULL DEFAULT 1,
                [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
                [last_update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_courses_classrooms FOREIGN KEY ([classroom_id]) REFERENCES [dbo].[classrooms]([id])
            );
        END;
        ELSE
        BEGIN
            IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.courses') AND name = 'classroom_id' AND is_nullable = 1)
            BEGIN
                UPDATE [dbo].[courses] SET [classroom_id] = 1 WHERE [classroom_id] IS NULL;
                ALTER TABLE [dbo].[courses] ALTER COLUMN [classroom_id] INT NOT NULL;
            END;

            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.courses') AND name = 'day_of_week')
            BEGIN
                ALTER TABLE [dbo].[courses]
                    ADD [day_of_week]  INT  NULL,
                        [start_time]   TIME NULL,
                        [end_time]     TIME NULL;
            END;
        END;

        -- =============================================
        -- 4. Table: course_teachers (N:N)
        -- =============================================
        IF OBJECT_ID('dbo.course_teachers', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[course_teachers] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [teacher_id]   INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_course_teachers_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_course_teachers_users   FOREIGN KEY ([teacher_id]) REFERENCES [dbo].[users]([id])
            );
        END;

        -- =============================================
        -- 5. Table: course_students (enrollments)
        -- =============================================
        IF OBJECT_ID('dbo.course_students', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[course_students] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [student_id]   INT      NOT NULL,
                [is_active]    BIT      NOT NULL DEFAULT 1,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_course_students_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_course_students_users   FOREIGN KEY ([student_id]) REFERENCES [dbo].[users]([id])
            );
        END;

        -- =============================================
        -- 6. Table: attendance
        -- =============================================
        IF OBJECT_ID('dbo.attendance', 'U') IS NULL
        BEGIN
            CREATE TABLE [dbo].[attendance] (
                [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                [course_id]    INT      NOT NULL,
                [student_id]   INT      NOT NULL,
                [date]         DATE     NOT NULL,
                [is_present]   BIT      NOT NULL DEFAULT 0,
                [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                CONSTRAINT fk_attendance_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                CONSTRAINT fk_attendance_users   FOREIGN KEY ([student_id]) REFERENCES [dbo].[users]([id]),
                CONSTRAINT uq_attendance_unique  UNIQUE ([course_id], [student_id], [date])
            );
        END;

        -- =============================================
        -- 7. Permissions
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_CURSO_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_CURSO_MGMT', 'ABM Cursos', 'Formulario de gestion de cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_INSCRIPCION_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_INSCRIPCION_MGMT', 'Inscripciones', 'Gestion de inscripciones de alumnos a cursos', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_ASISTENCIA_MGMT')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_MGMT', 'Asistencia', 'Registro de asistencia de alumnos en clases', 0);
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[permissions] WHERE [name] = 'FORM_ASISTENCIA_VIEW')
        BEGIN
            INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
            ('FORM_ASISTENCIA_VIEW', 'Ver Asistencia', 'Consulta de asistencia de alumnos', 0);
        END;

        -- =============================================
        -- 8. Role: Coordinador
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[roles] WHERE [name] = 'Coordinador')
        BEGIN
            INSERT INTO [dbo].[roles] (name) VALUES ('Coordinador');
        END;

        -- =============================================
        -- 9. Permission assignments
        -- =============================================

        -- Admin (1)
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 1 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 1, id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        -- Coordinador (4)
        DECLARE @coordinadorId INT = (SELECT id FROM [dbo].[roles] WHERE name = 'Coordinador');
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = @coordinadorId)
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT @coordinadorId, id FROM [dbo].[permissions]
            WHERE name IN ('FORM_USER_MGMT', 'FORM_ROLE_MGMT', 'FORM_CURSO_MGMT', 'FORM_INSCRIPCION_MGMT', 'FORM_COMPLAINTS', 'FORM_REPORTS', 'FORM_ASISTENCIA_VIEW');
        END;

        -- Profesor (2)
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_CURSO_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 2 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 2, id FROM [dbo].[permissions] WHERE name = 'FORM_ASISTENCIA_MGMT';
        END;

        DELETE FROM [dbo].[role_permissions]
        WHERE [role_id] = 2
          AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_USER_MGMT');

        -- Alumno (3)
        IF NOT EXISTS (SELECT 1 FROM [dbo].[role_permissions] WHERE [role_id] = 3 AND [permission_id] = (SELECT id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT'))
        BEGIN
            INSERT INTO [dbo].[role_permissions] (role_id, permission_id)
            SELECT 3, id FROM [dbo].[permissions] WHERE name = 'FORM_INSCRIPCION_MGMT';
        END;

        -- =============================================
        -- 10. Seed: classrooms
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[classrooms])
        BEGIN
            INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
            ('Aula 101 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 102 - Goiescalza',      30, 1, GETDATE(), GETDATE()),
            ('Aula 201 - Goiescalza',      25, 1, GETDATE(), GETDATE()),
            ('Laboratorio 1 - Goiescalza', 20, 1, GETDATE(), GETDATE()),
            ('Aula Magna - Goiescalza',   100, 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 11. Seed: courses with schedules
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[courses])
        BEGIN
            INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
            ('Ingles Basico A1',     'Curso de ingles para principiantes, nivel A1 del MCER',             '2026-08-01', '2026-12-15', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
            ('Ingles Intermedio B1', 'Curso de ingles nivel intermedio, preparacion para certificacion',  '2026-08-01', '2026-12-15', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
            ('Frances Inicial A1',   'Introduccion al frances, alfabetizacion y frases basicas',         '2026-08-01', '2026-12-15', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
            ('Portugues Basico',     'Curso de portugues para hispanohablantes, nivel inicial',           '2026-08-01', '2026-12-15', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
            ('Italiano A1',          'Curso de italiano para principantes, conversacion y gramatica',     '2026-08-01', '2026-12-15', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());
        END;

        -- =============================================
        -- 12. Seed: course_teachers
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[course_teachers])
        BEGIN
            INSERT INTO [dbo].[course_teachers] (course_id, teacher_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                (1, 'prof_garcia'),
                (1, 'prof_lopez'),
                (2, 'prof_martinez'),
                (3, 'prof_rodriguez'),
                (4, 'prof_fernandez'),
                (4, 'prof_garcia'),
                (5, 'prof_lopez')
            ) AS t(course_num, user_name)
            INNER JOIN [dbo].[courses] c ON c.id = t.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = t.user_name;
        END;

        -- =============================================
        -- 13. Seed: course_students (enrollments)
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[course_students])
        BEGIN
            INSERT INTO [dbo].[course_students] (course_id, student_id, is_active, created_at)
            SELECT c.id, u.id, 1, GETDATE()
            FROM (VALUES
                (1, 'alumno_perez'),
                (1, 'alumno_gomez'),
                (1, 'alumno_diaz'),
                (1, 'alumno_morales'),
                (1, 'alumno_torres'),
                (2, 'alumno_ramos'),
                (2, 'alumno_silva'),
                (2, 'alumno_castro'),
                (3, 'alumno_ruiz'),
                (3, 'alumno_alvarez'),
                (3, 'alumno_romero'),
                (3, 'alumno_sanchez'),
                (4, 'alumno_herrera'),
                (4, 'alumno_patricio'),
                (4, 'alumno_soto'),
                (5, 'alumno_perez'),
                (5, 'alumno_ruiz')
            ) AS e(course_num, user_name)
            INNER JOIN [dbo].[courses] c ON c.id = e.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = e.user_name;
        END;

        -- =============================================
        -- 14. Seed: attendance records
        -- =============================================
        IF NOT EXISTS (SELECT 1 FROM [dbo].[attendance])
        BEGIN
            INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
            SELECT c.id, u.id, a.date, a.is_present, GETDATE()
            FROM (VALUES
                (1, 'alumno_perez',   '2026-08-03', 1),
                (1, 'alumno_gomez',   '2026-08-03', 1),
                (1, 'alumno_diaz',    '2026-08-03', 0),
                (1, 'alumno_morales', '2026-08-03', 1),
                (1, 'alumno_torres',  '2026-08-03', 1),
                (1, 'alumno_perez',   '2026-08-10', 1),
                (1, 'alumno_gomez',   '2026-08-10', 0),
                (1, 'alumno_diaz',    '2026-08-10', 1),
                (1, 'alumno_morales', '2026-08-10', 1),
                (1, 'alumno_torres',  '2026-08-10', 1),
                (2, 'alumno_ramos',   '2026-08-05', 1),
                (2, 'alumno_silva',   '2026-08-05', 1),
                (2, 'alumno_castro',  '2026-08-05', 0),
                (2, 'alumno_ramos',   '2026-08-12', 1),
                (2, 'alumno_silva',   '2026-08-12', 0),
                (2, 'alumno_castro',  '2026-08-12', 1),
                (3, 'alumno_ruiz',      '2026-08-04', 1),
                (3, 'alumno_alvarez',   '2026-08-04', 1),
                (3, 'alumno_romero',    '2026-08-04', 1),
                (3, 'alumno_sanchez',   '2026-08-04', 0),
                (4, 'alumno_herrera',   '2026-08-06', 1),
                (4, 'alumno_patricio',  '2026-08-06', 0),
                (4, 'alumno_soto',      '2026-08-06', 1),
                (5, 'alumno_perez', '2026-08-07', 1),
                (5, 'alumno_ruiz',  '2026-08-07', 1)
            ) AS a(course_num, user_name, date, is_present)
            INNER JOIN [dbo].[courses] c ON c.id = a.course_num
            INNER JOIN [dbo].[users] u ON u.user_name = a.user_name;
        END;

        -- =============================================
        -- 15. Register migration
        -- =============================================
        INSERT INTO [dbo].[schema_versions] (version, script_name, applied_at)
        VALUES ('1.1.0', '03_v1.1.0_AulasCursosInscripciones.sql', GETDATE());

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
