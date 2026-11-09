USE DarioZubaray_TF;
GO

-- =============================================
-- SEED DEMO v1.0.0: Datos de prueba
--
-- Requiere haber corrido 01_v1.0.0_CreateTables.sql
-- (roles, permisos y usuario admin por defecto).
--
-- Carga usuarios de prueba, alumnos, aulas, cursos,
-- inscripciones, asistencia semanal, evaluaciones y
-- notas, para que los informes del director
-- (rendimiento, asistencia y notas por curso) tengan
-- datos desde la primera carga.
--
-- Idempotente por bloque: puede re-ejecutarse.
-- =============================================

-- Usuario dev (rol 1)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'dario')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('dario',  '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 1, 'es', 'System');

-- Coordinadores (rol 2)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'coord1')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('coord1', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('coord2', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System'),
    ('coord3', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 2, 'es', 'System');

-- Profesores (rol 3)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'prof_brooks')
    INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('prof_brooks',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_omh',      '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_conway',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_pareto',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_amdahl',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_kirchhoff','$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_murphy',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System'),
    ('prof_newton',   '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy',  1, 0, GETDATE(), GETDATE(), 3, 'es', 'System');

-- Director (rol 4)
IF NOT EXISTS (SELECT 1 FROM [dbo].[users] WHERE [user_name] = 'dir_schwarzenegger')
	INSERT INTO [dbo].[users] (user_name, password_hash, is_active, retries_count, last_update, created_at, role_id, language, theme) VALUES
    ('dir_schwarzenegger', '$2a$11$W5VIDAKnapRa9s7EksbNresgKwgSIgse6G5eJyt2MeErQOEji5Czy', 1, 0, GETDATE(), GETDATE(), 4, 'es', 'System');

-- =============================================
-- DVH: backfill del digito verificador 
-- =============================================
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL
BEGIN
    UPDATE u
    SET u.[dvh] = LOWER(CONVERT(VARCHAR(64),
        HASHBYTES('SHA2_256',
            CONCAT(
                CONVERT(NVARCHAR(MAX), u.[id]),          ',',
                u.[user_name],                           ',',
                u.[password_hash],                       ',',
                CASE WHEN u.[is_active] = 1 THEN '1' ELSE '0' END, ',',
                CONVERT(NVARCHAR(MAX), u.[retries_count]), ',',
                CONVERT(NVARCHAR(30), u.[last_update], 126), ',',
                CONVERT(NVARCHAR(30), u.[created_at], 126),  ',',
                CASE WHEN u.[role_id] IS NULL THEN '' ELSE CONVERT(NVARCHAR(MAX), u.[role_id]) END, ',',
                u.[language],                            ',',
                u.[theme]
            )
        ), 2))
    FROM [dbo].[users] u
    WHERE u.[dvh] IS NULL;
END
GO

-- =============================================
-- INSERT: alumnos 
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[students])
BEGIN
    SET IDENTITY_INSERT [dbo].[students] ON;

    INSERT INTO [dbo].[students] ([id], [first_name], [last_name], [document], [email], [phone], [is_active], [created_at], [last_update]) VALUES
    (1, 'alumno', 'juan',        'DNI-10200301', NULL, NULL, 1, GETDATE(), GETDATE()),
    (2, 'alumno', 'matias',      'DNI-10200302', NULL, NULL, 1, GETDATE(), GETDATE()),
    (3, 'alumno', 'pedro',       'DNI-10200303', NULL, NULL, 1, GETDATE(), GETDATE()),
    (4, 'alumno', 'tomás',       'DNI-10200304', NULL, NULL, 1, GETDATE(), GETDATE()),
    (5, 'alumno', 'gerardo',     'DNI-10200305', NULL, NULL, 1, GETDATE(), GETDATE()),
    (6, 'alumno', 'lucia',       'DNI-10200306', NULL, NULL, 1, GETDATE(), GETDATE()),
    (7, 'alumno', 'silvia',      'DNI-10200307', NULL, NULL, 1, GETDATE(), GETDATE()),
    (8, 'alumno', 'romina',      'DNI-10200308', NULL, NULL, 1, GETDATE(), GETDATE()),
    (9, 'alumno', 'ana',         'DNI-10200309', NULL, NULL, 1, GETDATE(), GETDATE()),
    (10, 'alumno', 'maria',      'DNI-10200310', NULL, NULL, 1, GETDATE(), GETDATE()),
    (11, 'alumno', 'antonela',   'DNI-10200311', NULL, NULL, 1, GETDATE(), GETDATE()),
    (12, 'alumno', 'julieta',    'DNI-10200312', NULL, NULL, 1, GETDATE(), GETDATE()),
    (13, 'alumno', 'agustín',    'DNI-10200313', NULL, NULL, 1, GETDATE(), GETDATE()),
    (14, 'alumno', 'benjamín',   'DNI-10200314', NULL, NULL, 1, GETDATE(), GETDATE()),
    (15, 'alumno', 'enzo',       'DNI-10200315', NULL, NULL, 1, GETDATE(), GETDATE());

    SET IDENTITY_INSERT [dbo].[students] OFF;
END

-- =============================================
-- INSERT: seed academico
-- =============================================

-- Aulas
IF NOT EXISTS (SELECT 1 FROM [dbo].[classrooms])
BEGIN
    INSERT INTO [dbo].[classrooms] (name, capacity, is_active, created_at, last_update) VALUES
    ('Aula 101',      30, 1, GETDATE(), GETDATE()),
    ('Aula 102',      30, 1, GETDATE(), GETDATE()),
    ('Aula 201',      25, 1, GETDATE(), GETDATE()),
    ('Laboratorio 1', 20, 1, GETDATE(), GETDATE()),
    ('Aula 202',     100, 1, GETDATE(), GETDATE());
END

-- Cursos con horario
IF NOT EXISTS (SELECT 1 FROM [dbo].[courses])
BEGIN
    INSERT INTO [dbo].[courses] (name, description, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update) VALUES
    ('Ingles A1',   'Curso de ingles para principiantes',        '2026-08-01', '2026-12-31', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE()),
    ('Ingles B1',   'Curso de ingles nivel intermedio',          '2026-08-01', '2026-12-31', 2, 3, '10:00', '12:00', 1, GETDATE(), GETDATE()),
    ('Frances A1',  'Introduccion al frances',                   '2026-08-01', '2026-12-31', 3, 2, '14:00', '16:00', 1, GETDATE(), GETDATE()),
    ('Portugues A1','Curso de portugues para hispanohablantes',  '2026-08-01', '2026-12-31', 4, 4, '18:00', '20:00', 1, GETDATE(), GETDATE()),
    ('Italiano A1', 'Curso de italiano para principantes',       '2026-08-01', '2026-12-31', 1, 5, '09:00', '11:00', 1, GETDATE(), GETDATE());
END

-- Profesor por curso
IF NOT EXISTS (SELECT 1 FROM [dbo].[course_teachers])
BEGIN
    INSERT INTO [dbo].[course_teachers] (course_id, teacher_id, is_active, created_at)
    SELECT c.id, u.id, 1, GETDATE()
    FROM (VALUES
    	(1, 'prof_brooks'),
    	(1, 'prof_omh'),
    	(2, 'prof_conway'),
    	(3, 'prof_pareto'),
    	(4, 'prof_amdahl'),
    	(4, 'prof_kirchhoff'),
    	(5, 'prof_murphy')
    ) AS t(course_num, user_name)
    INNER JOIN [dbo].[courses] c ON c.id = t.course_num
    INNER JOIN [dbo].[users] u ON u.user_name = t.user_name;
END

-- Inscripciones
IF NOT EXISTS (SELECT 1 FROM [dbo].[course_students])
BEGIN
    INSERT INTO [dbo].[course_students] (course_id, student_id, is_active, created_at)
    SELECT c.id, s.id, 1, GETDATE()
    FROM (VALUES
    	(1, 'alumno_juan'),
    	(1, 'alumno_matias'),
    	(1, 'alumno_pedro'),
    	(1, 'alumno_tomás'),
    	(1, 'alumno_gerardo'),
    	(2, 'alumno_lucia'),
    	(2, 'alumno_silvia'),
    	(2, 'alumno_romina'),
    	(3, 'alumno_ana'),
    	(3, 'alumno_maria'),
    	(3, 'alumno_antonela'),
    	(3, 'alumno_julieta'),
    	(4, 'alumno_agustín'),
    	(4, 'alumno_benjamín'),
    	(4, 'alumno_enzo'),
    	(5, 'alumno_juan'),
    	(5, 'alumno_maria')
    ) AS e(course_num, student_name)
    INNER JOIN [dbo].[courses] c ON c.id = e.course_num
    INNER JOIN [dbo].[students] s ON (s.[first_name] + '_' + s.[last_name]) = e.student_name;
END

-- Asistencia (sesiones iniciales)
IF NOT EXISTS (SELECT 1 FROM [dbo].[attendance])
BEGIN
    INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
    SELECT c.id, s.id, a.date, a.is_present, GETDATE()
    FROM (VALUES
    	(1, 'alumno_juan',     '2026-08-03', 1),
    	(1, 'alumno_matias',   '2026-08-03', 1),
    	(1, 'alumno_pedro',    '2026-08-03', 0),
    	(1, 'alumno_tomás',    '2026-08-03', 1),
    	(1, 'alumno_gerardo',  '2026-08-03', 1),
    	(1, 'alumno_juan',     '2026-08-10', 1),
    	(1, 'alumno_matias',   '2026-08-10', 0),
    	(1, 'alumno_pedro',    '2026-08-10', 1),
    	(1, 'alumno_tomás',    '2026-08-10', 1),
    	(1, 'alumno_gerardo',  '2026-08-10', 1),
    	(2, 'alumno_lucia',    '2026-08-05', 1),
    	(2, 'alumno_silvia',   '2026-08-05', 1),
    	(2, 'alumno_romina',   '2026-08-05', 0),
    	(2, 'alumno_lucia',    '2026-08-12', 1),
    	(2, 'alumno_silvia',   '2026-08-12', 0),
    	(2, 'alumno_romina',   '2026-08-12', 1),
    	(3, 'alumno_ana',      '2026-08-04', 1),
    	(3, 'alumno_maria',    '2026-08-04', 1),
    	(3, 'alumno_antonela', '2026-08-04', 1),
    	(3, 'alumno_julieta',  '2026-08-04', 0),
    	(4, 'alumno_agustín',  '2026-08-06', 1),
    	(4, 'alumno_benjamín', '2026-08-06', 0),
    	(4, 'alumno_enzo',     '2026-08-06', 1),
    	(5, 'alumno_juan',     '2026-08-07', 1),
    	(5, 'alumno_ana',      '2026-08-07', 1)
    ) AS a(course_num, student_name, date, is_present)
    INNER JOIN [dbo].[courses] c ON c.id = a.course_num
    INNER JOIN [dbo].[students] s ON (s.[first_name] + '_' + s.[last_name]) = a.student_name;
END

-- =============================================
-- INSERT: asistencia - sesiones semanales (ago-oct)
-- 8 fechas por curso, alineadas al dia del curso y
-- dentro del rango 2026-08-01 .. 2026-12-31.
-- Presencia deterministica (~80%) por alumno y fecha.
-- =============================================
;WITH fechas AS (
    SELECT 1 AS course_num, CAST('2026-08-17' AS DATE) AS d UNION ALL
    SELECT 1, '2026-08-24' UNION ALL
    SELECT 1, '2026-08-31' UNION ALL
    SELECT 1, '2026-09-07' UNION ALL
    SELECT 1, '2026-09-14' UNION ALL
    SELECT 1, '2026-09-21' UNION ALL
    SELECT 1, '2026-09-28' UNION ALL
    SELECT 1, '2026-10-05' UNION ALL
    SELECT 2, '2026-08-19' UNION ALL
    SELECT 2, '2026-08-26' UNION ALL
    SELECT 2, '2026-09-02' UNION ALL
    SELECT 2, '2026-09-09' UNION ALL
    SELECT 2, '2026-09-16' UNION ALL
    SELECT 2, '2026-09-23' UNION ALL
    SELECT 2, '2026-09-30' UNION ALL
    SELECT 2, '2026-10-07' UNION ALL
    SELECT 3, '2026-08-11' UNION ALL
    SELECT 3, '2026-08-18' UNION ALL
    SELECT 3, '2026-08-25' UNION ALL
    SELECT 3, '2026-09-01' UNION ALL
    SELECT 3, '2026-09-08' UNION ALL
    SELECT 3, '2026-09-15' UNION ALL
    SELECT 3, '2026-09-22' UNION ALL
    SELECT 3, '2026-09-29' UNION ALL
    SELECT 4, '2026-08-13' UNION ALL
    SELECT 4, '2026-08-20' UNION ALL
    SELECT 4, '2026-08-27' UNION ALL
    SELECT 4, '2026-09-03' UNION ALL
    SELECT 4, '2026-09-10' UNION ALL
    SELECT 4, '2026-09-17' UNION ALL
    SELECT 4, '2026-09-24' UNION ALL
    SELECT 4, '2026-10-01' UNION ALL
    SELECT 5, '2026-08-14' UNION ALL
    SELECT 5, '2026-08-21' UNION ALL
    SELECT 5, '2026-08-28' UNION ALL
    SELECT 5, '2026-09-04' UNION ALL
    SELECT 5, '2026-09-11' UNION ALL
    SELECT 5, '2026-09-18' UNION ALL
    SELECT 5, '2026-09-25' UNION ALL
    SELECT 5, '2026-10-02'
)
INSERT INTO [dbo].[attendance] (course_id, student_id, date, is_present, created_at)
SELECT cs.course_id, cs.student_id, f.d,
       CASE WHEN ((cs.student_id * 7 + cs.course_id * 3 + DATEDIFF(DAY, '2026-01-01', f.d)) % 10) < 8
            THEN 1 ELSE 0 END,
       GETDATE()
FROM fechas f
INNER JOIN [dbo].[course_students] cs
        ON cs.course_id = f.course_num AND cs.is_active = 1
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[attendance] a
    WHERE a.course_id = cs.course_id
      AND a.student_id = cs.student_id
      AND a.date = f.d
);

-- =============================================
-- INSERT: evaluaciones (3 por curso)
-- Creadas por el docente del curso, en el dia del
-- curso y dentro del rango 2026-08-01 .. 2026-12-31.
-- =============================================
INSERT INTO [dbo].[evaluations] (course_id, name, date, created_by, created_at, last_update)
SELECT c.id, v.name, CAST(v.date AS DATE), u.id, GETDATE(), GETDATE()
FROM (VALUES
    (1, 'Parcial 1', '2026-09-14', 'prof_brooks'),
    (1, 'Parcial 2', '2026-10-19', 'prof_brooks'),
    (1, 'Final',      '2026-12-14', 'prof_brooks'),
    (2, 'Parcial 1', '2026-09-16', 'prof_conway'),
    (2, 'Parcial 2', '2026-10-21', 'prof_conway'),
    (2, 'Final',      '2026-12-16', 'prof_conway'),
    (3, 'Parcial 1', '2026-09-15', 'prof_pareto'),
    (3, 'Parcial 2', '2026-10-20', 'prof_pareto'),
    (3, 'Final',      '2026-12-15', 'prof_pareto'),
    (4, 'Parcial 1', '2026-09-17', 'prof_amdahl'),
    (4, 'Parcial 2', '2026-10-22', 'prof_amdahl'),
    (4, 'Final',      '2026-12-17', 'prof_amdahl'),
    (5, 'Parcial 1', '2026-09-18', 'prof_murphy'),
    (5, 'Parcial 2', '2026-10-23', 'prof_murphy'),
    (5, 'Final',      '2026-12-18', 'prof_murphy')
) AS v(course_num, name, date, teacher)
INNER JOIN [dbo].[courses] c ON c.id = v.course_num
INNER JOIN [dbo].[users] u ON u.user_name = v.teacher
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[evaluations] e
    WHERE e.course_id = c.id
      AND e.name = v.name
);

-- =============================================
-- INSERT: notas - una por alumno inscripto y evaluacion
-- Rango 4.00 a 10.00 (respeta ck_grades_range).
-- Se omiten 4 notas para mostrar alumnos con menos
-- notas que cantidad de evaluaciones en los informes.
-- =============================================
INSERT INTO [dbo].[grades] (evaluation_id, student_id, grade)
SELECT e.id, cs.student_id,
       CAST(4 + ((e.id * 13 + cs.student_id * 7) % 61) / 10.0 AS DECIMAL(4,2))
FROM [dbo].[evaluations] e
INNER JOIN [dbo].[course_students] cs
        ON cs.course_id = e.course_id AND cs.is_active = 1
WHERE e.name IN ('Parcial 1', 'Parcial 2', 'Final')
  AND NOT (
        (e.name = 'Parcial 1' AND cs.student_id IN (3, 11)) OR
        (e.name = 'Final'     AND cs.student_id IN (8, 14))
  )
  AND NOT EXISTS (
        SELECT 1 FROM [dbo].[grades] g
        WHERE g.evaluation_id = e.id
          AND g.student_id = cs.student_id
  );

-- Ticket
IF NOT EXISTS (SELECT 1 FROM [dbo].[tickets])
BEGIN
    INSERT INTO [dbo].[tickets] (user_id, subject, description, is_closed, created_at, closed_at, closed_by) VALUES
    (2, 'Open Ticket', 'This is an example of an open ticket created', 0, GETDATE(), NULL, NULL),
    (2, 'Closed ticket', 'this ticket was closed due to missing information', 1, GETDATE(), GETDATE(), 1);
END
