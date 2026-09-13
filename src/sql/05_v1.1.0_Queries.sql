use Trabajo_Final;

-- =============================================
-- QUERIES: Consultas utiles para desarrollo
-- =============================================

-- Users, Roles, Permissions
SELECT u.id, u.user_name, r.name AS rol, u.language, u.is_active
FROM users u
LEFT JOIN roles r ON r.id = u.role_id;

SELECT r.name AS rol, p.name AS permiso, p.label AS formulario
FROM role_permissions rp
INNER JOIN roles r ON r.id = rp.role_id
INNER JOIN permissions p ON p.id = rp.permission_id
ORDER BY r.name, p.name;

SELECT r1.name AS padre, r2.name AS hijo
FROM role_hierarchy rh
INNER JOIN roles r1 ON r1.id = rh.parent_role_id
INNER JOIN roles r2 ON r2.id = rh.child_role_id;

SELECT r.name AS rol, COUNT(u.id) AS cantidad
FROM roles r
LEFT JOIN users u ON u.role_id = r.id
GROUP BY r.name;

-- Classrooms, Courses
SELECT id, name, capacity, is_active FROM classrooms WHERE is_active = 1;

SELECT c.id, c.name, c.description, c.start_date, c.end_date,
       c.day_of_week, c.start_time, c.end_time,
       cl.name AS classroom,
       STRING_AGG(u.user_name, ', ') AS teachers
FROM courses c
LEFT JOIN classrooms cl ON cl.id = c.classroom_id
LEFT JOIN course_teachers ct ON ct.course_id = c.id AND ct.is_active = 1
LEFT JOIN users u ON u.id = ct.teacher_id
WHERE c.is_active = 1
GROUP BY c.id, c.name, c.description, c.start_date, c.end_date,
         c.day_of_week, c.start_time, c.end_time, cl.name;

-- Enrollments
SELECT cs.id, u.user_name AS student, c.name AS course
FROM course_students cs
INNER JOIN users u ON u.id = cs.student_id
INNER JOIN courses c ON c.id = cs.course_id
WHERE cs.is_active = 1 AND c.is_active = 1
ORDER BY u.user_name, c.name;

-- Attendance
SELECT u.user_name AS student, a.is_present, a.date
FROM attendance a
INNER JOIN users u ON u.id = a.student_id
WHERE a.course_id = 1 AND a.date = '2026-08-03'
ORDER BY u.user_name;

-- Migration history
SELECT * FROM schema_versions ORDER BY applied_at DESC;
