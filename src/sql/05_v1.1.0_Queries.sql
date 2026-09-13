use Trabajo_Final;

-- =============================================
-- QUERIES: Consultas utiles para desarrollo
-- =============================================

-- =============================================
-- v1.0.0: Usuarios, Roles y Permisos
-- =============================================

-- Ver todos los usuarios con su rol
SELECT u.id, u.user_name, r.name AS rol, u.language, u.is_active
FROM Users u
LEFT JOIN Roles r ON r.id = u.role_id;

-- Ver permisos de cada rol
SELECT r.name AS rol, p.name AS permiso, p.label AS formulario
FROM RolePermissions rp
INNER JOIN Roles r ON r.id = rp.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
ORDER BY r.name, p.name;

-- Ver jerarquia de roles
SELECT r1.name AS padre, r2.name AS hijo
FROM RoleHierarchy rh
INNER JOIN Roles r1 ON r1.id = rh.parent_role_id
INNER JOIN Roles r2 ON r2.id = rh.child_role_id;

-- Verificar si un usuario tiene permiso especifico
SELECT u.user_name, p.name AS permiso
FROM Users u
INNER JOIN RolePermissions rp ON rp.role_id = u.role_id
INNER JOIN Permissions p ON p.id = rp.permission_id
WHERE u.user_name = 'admin' AND p.name = 'FORM_CURSO_MGMT';

-- Contar usuarios por rol
SELECT r.name AS rol, COUNT(u.id) AS cantidad
FROM Roles r
LEFT JOIN Users u ON u.role_id = r.id
GROUP BY r.name;

-- =============================================
-- v1.1.0: Aulas, Cursos, Inscripciones
-- =============================================

-- Ver todas las aulas activas
SELECT id, nombre, capacidad, is_active FROM Aulas WHERE is_active = 1;

-- Ver cursos con su aula y docentes
SELECT c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
       c.dia_semana, c.hora_inicio, c.hora_fin,
       a.nombre AS aula,
       STRING_AGG(u.user_name, ', ') AS docentes
FROM Cursos c
LEFT JOIN Aulas a ON a.id = c.aula_id
LEFT JOIN CursoDocentes cd ON cd.curso_id = c.id AND cd.is_active = 1
LEFT JOIN Users u ON u.id = cd.docente_id
WHERE c.is_active = 1
GROUP BY c.id, c.nombre, c.descripcion, c.fecha_inicio, c.fecha_fin,
         c.dia_semana, c.hora_inicio, c.hora_fin, a.nombre;

-- Ver inscripciones activas
SELECT ca.id, u.user_name AS alumno, c.nombre AS curso,
       c.dia_semana, c.hora_inicio, c.hora_fin,
       a.nombre AS aula
FROM CursoAlumnos ca
INNER JOIN Users u ON u.id = ca.alumno_id
INNER JOIN Cursos c ON c.id = ca.curso_id
LEFT JOIN Aulas a ON a.id = c.aula_id
WHERE ca.is_active = 1 AND c.is_active = 1
ORDER BY u.user_name, c.nombre;

-- Contar alumnos inscriptos por curso
SELECT c.nombre AS curso, COUNT(ca.id) AS alumnos_inscriptos
FROM Cursos c
LEFT JOIN CursoAlumnos ca ON ca.curso_id = c.id AND ca.is_active = 1
WHERE c.is_active = 1
GROUP BY c.nombre;

-- =============================================
-- v1.2.0: Asistencia
-- =============================================

-- Ver asistencia de un curso en una fecha
SELECT u.user_name AS alumno, cl.presente, cl.fecha
FROM ClasesAlumnos cl
INNER JOIN Users u ON u.id = cl.alumno_id
WHERE cl.curso_id = 1 AND cl.fecha = '2026-08-03'
ORDER BY u.user_name;

-- Porcentaje de asistencia por alumno en un curso
SELECT u.user_name AS alumno,
       COUNT(CASE WHEN cl.presente = 1 THEN 1 END) AS presente,
       COUNT(*) AS total_clases,
       CAST(COUNT(CASE WHEN cl.presente = 1 THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,1)) AS porcentaje
FROM ClasesAlumnos cl
INNER JOIN Users u ON u.id = cl.alumno_id
WHERE cl.curso_id = 1
GROUP BY u.user_name;

-- Verificar traslape de aula (ejemplo: buscar conflictos para aula 1 en agosto-dic 2026)
SELECT c.nombre, c.fecha_inicio, c.fecha_fin
FROM Cursos c
WHERE c.aula_id = 1
  AND c.is_active = 1
  AND c.fecha_inicio < '2026-12-15'
  AND c.fecha_fin > '2026-08-01';

-- Verificar migraciones aplicadas
SELECT * FROM SchemaVersions ORDER BY AppliedAt DESC;
