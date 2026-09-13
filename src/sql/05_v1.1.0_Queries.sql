use Trabajo_Final;

-- =============================================
-- QUERIES: Consultas utiles para desarrollo
-- =============================================

-- =============================================
-- v1.0.0: Users, Roles, Permissions
-- =============================================

SELECT u.id, u.user_name, r.name AS rol, u.language, u.is_active
FROM Users u
LEFT JOIN Roles r ON r.id = u.role_id;

SELECT r.name AS rol, p.name AS permiso, p.label AS formulario
FROM RolePermissions rp
INNER JOIN Roles r ON r.id = rp.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
ORDER BY r.name, p.name;

SELECT r1.name AS padre, r2.name AS hijo
FROM RoleHierarchy rh
INNER JOIN Roles r1 ON r1.id = rh.parent_role_id
INNER JOIN Roles r2 ON r2.id = rh.child_role_id;

SELECT u.user_name, p.name AS permiso
FROM Users u
INNER JOIN RolePermissions rp ON rp.role_id = u.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
WHERE u.user_name = 'admin' AND p.name = 'FORM_CURSO_MGMT';

SELECT r.name AS rol, COUNT(u.id) AS cantidad
FROM Roles r
LEFT JOIN Users u ON u.role_id = r.id
GROUP BY r.name;

-- =============================================
-- v1.1.0: Classrooms, Courses, Enrollments
-- =============================================

SELECT id, name, capacity, is_active FROM classrooms WHERE is_active = 1;

SELECT c.id, c.name, c.description, c.start_date, c.end_date,
       c.day_of_week, c.start_time, c.end_time,
       cl.name AS classroom,
       STRING_AGG(u.user_name, ', ') AS teachers
FROM courses c
LEFT JOIN classrooms cl ON cl.id = c.classroom_id
LEFT JOIN course_teachers ct ON ct.course_id = c.id AND ct.is_active = 1
LEFT JOIN Users u ON u.id = ct.teacher_id
WHERE c.is_active = 1
GROUP BY c.id, c.name, c.description, c.start_date, c.end_date,
         c.day_of_week, c.start_time, c.end_time, cl.name;

SELECT cs.id, u.user_name AS student, c.name AS course,
       c.day_of_week, c.start_time, c.end_time,
       cl.name AS classroom
FROM course_students cs
INNER JOIN Users u ON u.id = cs.student_id
INNER JOIN courses c ON c.id = cs.course_id
LEFT JOIN classrooms cl ON cl.id = c.classroom_id
WHERE cs.is_active = 1 AND c.is_active = 1
ORDER BY u.user_name, c.name;

SELECT c.name AS course, COUNT(cs.id) AS enrolled
FROM courses c
LEFT JOIN course_students cs ON cs.course_id = c.id AND cs.is_active = 1
WHERE c.is_active = 1
GROUP BY c.name;

-- =============================================
-- v1.2.0: Attendance
-- =============================================

SELECT u.user_name AS student, a.is_present, a.date
FROM attendance a
INNER JOIN Users u ON u.id = a.student_id
WHERE a.course_id = 1 AND a.date = '2026-08-03'
ORDER BY u.user_name;

SELECT u.user_name AS student,
       COUNT(CASE WHEN a.is_present = 1 THEN 1 END) AS present,
       COUNT(*) AS total,
       CAST(COUNT(CASE WHEN a.is_present = 1 THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,1)) AS pct
FROM attendance a
INNER JOIN Users u ON u.id = a.student_id
WHERE a.course_id = 1
GROUP BY u.user_name;

SELECT * FROM SchemaVersions ORDER BY AppliedAt DESC;
