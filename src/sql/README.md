# Scripts SQL (`src/sql/`)

Scripts para crear, poblar y consultar la base de datos **`DarioZubaray_TF`** (SQL Server).

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [`00_v0.0.1_PurgeDatabase.sql`](00_v0.0.1_PurgeDatabase.sql) | Elimina todas las tablas (reinicialización limpia). |
| [`01_v1.0.0_CreateTables.sql`](01_v1.0.0_CreateTables.sql) | Esquema base: Roles, Permissions, RolePermissions, RoleHierarchy, Users, ActivityLogs, SchemaVersions. |
| [`02_v1.0.0_SeedData.sql`](02_v1.0.0_SeedData.sql) | Datos iniciales: roles, permisos, 24 usuarios de prueba (los alumnos con `is_active = 0`: son datos, no actores del sistema). |
| [`03_v1.1.0_AulasCursosInscripciones.sql`](03_v1.1.0_AulasCursosInscripciones.sql) | Migración consolidada: classrooms, courses, course_teachers, course_students, attendance. Permisos, rol Coordinador. Seed data completo. |
| [`04_v1.1.0_Rollback.sql`](04_v1.1.0_Rollback.sql) | Rollback de v1.1.0. |
| [`05_v1.1.0_Queries.sql`](05_v1.1.0_Queries.sql) | Consultas de verificación y desarrollo. |
| [`06_v1.1.0_ScriptConsolidado.sql`](06_v1.1.0_ScriptConsolidado.sql) | **Script maestro**: crea la base (si no existe), crea todas las tablas, inserta los datos y registra las versiones en un solo paso. |
| [`07_v1.2.0_AdminPassword.sql`](07_v1.2.0_AdminPassword.sql) | Migración v1.2.0: cambia la contraseña del usuario `admin` a `admin` y resetea su estado (exento de bloqueo, 2FA). |
| [`08_v1.3.0_DVH.sql`](08_v1.3.0_DVH.sql) | Migración v1.3.0: agrega la columna `users.dvh` (Dígito Verificador Horizontal) y calcula el hash SHA-256 de cada fila (columnas unidas por coma, sin espacios). |
| [`09_v1.4.0_Tickets.sql`](09_v1.4.0_Tickets.sql) | Migración v1.4.0: crea la tabla `tickets` (reportes de usuarios con estados Abierto/Cerrado) y el permiso `FORM_TICKET_MGMT` (gestión administrativa, rol Admin). |
| [`10_v1.4.1_AjusteActores.sql`](10_v1.4.1_AjusteActores.sql) | Migración v1.4.1: desactiva el login de los alumnos (`is_active = 0`; siguen como datos para inscripción/asistencia) y otorga `FORM_TICKET_MGMT` al rol Coordinador (listar todos los tickets y cerrarlos; Profesor solo crea). |
| [`11_v1.5.0_EstudiantesYRoles.sql`](11_v1.5.0_EstudiantesYRoles.sql) | Migración v1.5.0: crea la tabla `students` (entidad propia de alumnos, ya no son usuarios), migra los alumnos desde `users`, repunta las FK de `course_students` y `attendance`, elimina el rol Alumno, crea el rol Director (`FORM_REPORTS`) y el permiso `FORM_STUDENT_MGMT`, y ajusta los permisos de Admin/Profesor/Coordinador. |
| [`12_v1.6.0_Evaluaciones.sql`](12_v1.6.0_Evaluaciones.sql) | Migración v1.6.0: crea las tablas `evaluations` y `grades` (notas 0–10 por alumno y evaluación) y el permiso `FORM_EVAL_MGMT` (menú Académico → Evaluaciones), otorgado al rol Profesor. |

## Instalación

### Opción A — Script maestro (recomendada)

1. Ejecutar [`06_v1.1.0_ScriptConsolidado.sql`](06_v1.1.0_ScriptConsolidado.sql). Crea la base `DarioZubaray_TF` si no existe, dropea las tablas existentes, crea el esquema consolidado (v1.0.0 + v1.1.0 + DVH v1.3.0 + tickets v1.4.0 + ajuste de actores v1.4.1) y carga todos los datos.
2. Ejecutar [`11_v1.5.0_EstudiantesYRoles.sql`](11_v1.5.0_EstudiantesYRoles.sql). Migra los alumnos a la tabla `students`, ajusta los roles finales (sin Alumno, con Director) y agrega el permiso `FORM_STUDENT_MGMT`.
3. Ejecutar [`12_v1.6.0_Evaluaciones.sql`](12_v1.6.0_Evaluaciones.sql). Crea `evaluations`/`grades` y el permiso `FORM_EVAL_MGMT` para el rol Profesor.

### Opción B — Scripts por pasos (migraciones)

```sql
-- 1. Reinicializar
00_v0.0.1_PurgeDatabase.sql

-- 2. Esquema base
01_v1.0.0_CreateTables.sql

-- 3. Datos iniciales
02_v1.0.0_SeedData.sql

-- 4. Migración v1.1.0 (aulas, cursos, inscripciones, asistencia)
03_v1.1.0_AulasCursosInscripciones.sql

-- 5. Migración v1.2.0 (contraseña admin/admin)
07_v1.2.0_AdminPassword.sql

-- 6. Migración v1.3.0 (DVH: columna users.dvh + backfill de hashes)
08_v1.3.0_DVH.sql

-- 7. Migración v1.4.0 (tickets: tabla + permiso FORM_TICKET_MGMT)
09_v1.4.0_Tickets.sql

-- 8. Migración v1.4.1 (alumnos solo datos + FORM_TICKET_MGMT al Coordinador)
10_v1.4.1_AjusteActores.sql

-- 9. Migración v1.5.0 (tabla students, rol Director, permiso FORM_STUDENT_MGMT)
11_v1.5.0_EstudiantesYRoles.sql

-- 10. Migración v1.6.0 (evaluations/grades + permiso FORM_EVAL_MGMT)
12_v1.6.0_Evaluaciones.sql

-- 11. (opcional) Consultas de verificación
05_v1.1.0_Queries.sql
```

## Convención de nombres

**Tablas v1.0.0** (PascalCase, se mantienen por compatibilidad):
`Roles`, `Permissions`, `RolePermissions`, `RoleHierarchy`, `Users`, `ActivityLogs`, `SchemaVersions`

**Tablas v1.1.0+** (snake_case plural inglés):
`classrooms`, `courses`, `course_teachers`, `course_students`, `attendance`, `tickets`, `students`, `evaluations`, `grades`

**Columnas**: Todas `snake_case` (`start_date`, `day_of_week`, `is_active`, etc.)

**Columna `users.dvh`**: Dígito Verificador Horizontal (SHA-256 en hex). Se regenera desde la aplicación en cada INSERT/UPDATE (la orquestación vive en `BLL.DVHHelperBLL`) y se valida al inicio de la aplicación. Debe calcularse con el mismo formato que `BLL.DVHHelperBLL` (valores en orden de columna unidos por `,`, fechas estilo `126` —sin fracción si ms = 0—, `role_id NULL` → vacío) y hashear el payload en **UTF-16LE**, igual que `HASHBYTES('SHA2_256', <NVARCHAR>)`.

## Usuarios de prueba

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `admin` | Admin — exento de bloqueo por 3 intentos; pide código 2FA `123` |
| `dario` | `123` | Admin |
| `coord_maria`, `coord_carlos`, `coord_laura` | `123` | Coordinador |
| `prof_garcia` a `prof_fernandez` (5) | `123` | Profesor |

> Los alumnos **no son usuarios ni actores del sistema**: se gestionan en la tabla `students` (menú Administración → Alumnos, permiso `FORM_STUDENT_MGMT`). Tras la migración v1.5.0, los alumnos seed (`alumno_perez` a `alumno_soto`) quedaron migrados a `students` con los mismos IDs.

## Cursos de prueba (Ago-Dic 2026)

| Curso | Aula | Día | Horario | Docentes | Alumnos |
|-------|------|-----|---------|----------|---------|
| Ingles Basico A1 | Aula 101 | Lunes | 08:00-10:00 | garcia, lopez | 5 |
| Ingles Intermedio B1 | Aula 102 | Miércoles | 10:00-12:00 | martinez | 3 |
| Frances Inicial A1 | Aula 201 | Martes | 14:00-16:00 | rodriguez | 4 |
| Portugues Basico | Lab 1 | Jueves | 18:00-20:00 | fernandez, garcia | 3 |
| Italiano A1 | Aula 101 | Viernes | 09:00-11:00 | lopez | 2 |
