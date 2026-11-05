# Scripts SQL (`src/sql/`)

Scripts para crear, poblar y consultar la base de datos **`DarioZubaray_TF`** (SQL Server).

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [`01_v1.0.0_CreateTables.sql`](01_v1.0.0_CreateTables.sql) | Esquema base: `roles`, `permissions`, `role_permissions`, `role_hierarchy`, `users`, `activity_logs`, `schema_versions`. |
| [`02_v1.0.0_SeedData.sql`](02_v1.0.0_SeedData.sql) | Datos iniciales: 4 roles, 6 permisos, 25 usuarios de prueba (15 alumnos con `is_active = 0`: son datos, no actores del sistema) y la versión `1.0.0` en `schema_versions`. |
| [`03_v1.3.0_Incremental.sql`](03_v1.3.0_Incremental.sql) | **Migración incremental a v1.3.0** (todo lo posterior a v1.0.0 en un solo paso idempotente): aulas/cursos/inscripciones/asistencia, permisos y rol Coordinador, DVH (`users.dvh` + backfill), tickets, alumnos pasan de `users` a la entidad `students` (rol Alumno eliminado, rol Director creado), `evaluations`/`grades`, permisos de informes, respaldo y alta de tickets. Registra `1.3.0` en `schema_versions`. |
| [`04_v1.3.0_Rollback.sql`](04_v1.3.0_Rollback.sql) | **Rollback de v1.3.0**: devuelve la base al estado v1 (esquema y seed de `01`+`02`). Ver notas abajo. |
| [`05_v1.3.0_ScriptConsolidado.sql`](05_v1.3.0_ScriptConsolidado.sql) | **Script maestro**: crea la base (si no existe), purga las tablas, crea el esquema completo (estado final v1.3.0), carga todos los datos y registra `1.0.0` y `1.3.0`. |

## Instalación

### Opción A — Script maestro (recomendada)

1. Ejecutar [`05_v1.3.0_ScriptConsolidado.sql`](05_v1.3.0_ScriptConsolidado.sql). Crea la base `DarioZubaray_TF` si no existe, la deja con el esquema y los datos finales (v1.3.0) en un solo paso.

### Opción B — Scripts por pasos (migraciones)

```sql
-- 1. Esquema base
01_v1.0.0_CreateTables.sql

-- 2. Datos iniciales (v1.0.0)
02_v1.0.0_SeedData.sql

-- 3. Migración incremental a v1.3.0
03_v1.3.0_Incremental.sql
```

Ambas opciones dejan la base en el mismo esquema (16 tablas) y con la misma matriz final de permisos por rol (18 permisos); `03_v1.3.0_Incremental.sql` es idempotente: su lote de migración no se re-ejecuta si `1.3.0` ya fue aplicada, y su lote final de alineación de permisos corre siempre sin duplicar filas. Los datos de prueba difieren entre ambas opciones (el seed de `02` conserva los usuarios y cursos originales; `05` trae el seed actualizado).

### Rollback

[`04_v1.3.0_Rollback.sql`](04_v1.3.0_Rollback.sql) revierte la migración v1.3.0: elimina las tablas agregadas (`grades`, `evaluations`, `tickets`, `attendance`, `course_students`, `course_teachers`, `courses`, `classrooms`, `students`), la columna `users.dvh`, los permisos agregados (incluidos los 3 base de Archivo/Ayuda) y restaura los flags `is_system` de `FORM_CHANGE_PASS`/`FORM_PREFERENCES`, el rol Director (sus usuarios pasan a Admin), el rol Alumno, la matriz de permisos y los 15 usuarios-alumno del seed v1. Registra `1.3.0-rollback` y libera la versión `1.3.0`.

> **Advertencia**: el rollback es destructivo (borra tablas y datos académicos) y deja la base incompatible con la aplicación actual. Para volver a usarla hay que re-ejecutar `03_v1.3.0_Incremental.sql` (el ciclo rollback → incremental es seguro y probado).

## Bases de datos legadas (cadenas anteriores)

Las bases creadas con las versiones previas de los scripts (hasta `14_v1.8.0_Respaldo.sql`) **no necesitan recrearse**: `03_v1.3.0_Incremental.sql` detecta la fila legada `1.3.0` (distinto `script_name`) y aplica solo los cambios pendientes, además de corregir la matriz de permisos del rol Coordinador (8 permisos).

## Convención de nombres

**Tablas v1.0.0** (PascalCase, se mantienen por compatibilidad):
`Roles`, `Permissions`, `RolePermissions`, `RoleHierarchy`, `Users`, `ActivityLogs`, `SchemaVersions`

**Tablas v1.1.0+** (snake_case plural inglés):
`classrooms`, `courses`, `course_teachers`, `course_students`, `attendance`, `tickets`, `students`, `evaluations`, `grades`

**Columnas**: Todas `snake_case` (`start_date`, `day_of_week`, `is_active`, etc.)

**Columna `users.dvh`**: Dígito Verificador Horizontal (SHA-256 en hex). Se regenera desde la aplicación en cada INSERT/UPDATE (la orquestación vive en `BLL.DVHHelperBLL`) y se valida al inicio de la aplicación. Debe calcularse con el mismo formato que `BLL.DVHHelperBLL` (valores en orden de columna unidos por `,`, fechas estilo `126` —sin fracción si ms = 0—, `role_id NULL` → vacío) y hashear el payload en **UTF-16LE**, igual que `HASHBYTES('SHA2_256', <NVARCHAR>)`.

## Matriz de permisos por rol (estado final v1.3.0 — 18 permisos)

| Rol | Permisos |
|-----|----------|
| Admin | todos excepto `FORM_TICKET_CREATE` (17) |
| Coordinador | `FORM_CURSO_MGMT`, `FORM_INSCRIPCION_MGMT`, `FORM_ASISTENCIA_VIEW`, `FORM_TICKET_MGMT`, `FORM_STUDENT_MGMT`, `FORM_TICKET_CREATE` + los 5 base (11) |
| Profesor | `FORM_ASISTENCIA_MGMT`, `FORM_EVAL_MGMT`, `FORM_TICKET_CREATE` + los 5 base (8) |
| Director | `FORM_REPORTS`, `FORM_REPORT_DETAIL` + los 5 base (7) |

Los 5 permisos base (menú Archivo y Ayuda, `is_system = 0`, otorgados a **todos** los roles): `FORM_PREFERENCES`, `FORM_CHANGE_PASS`, `FORM_ACTIVITY_HISTORY`, `FORM_LOGOUT`, `FORM_ABOUT`.

## Usuarios de prueba (seed de `05`)

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `admin` | Admin — exento de bloqueo por 3 intentos; pide código 2FA `123` |
| `dario` | `123` | Admin |
| `coord1`, `coord2`, `coord3` | `123` | Coordinador |
| `prof_brooks`, `prof_omh`, `prof_conway`, `prof_pareto`, `prof_amdahl`, `prof_kirchhoff`, `prof_murphy`, `prof_newton` | `123` | Profesor |

> Los alumnos **no son usuarios ni actores del sistema**: se gestionan en la tabla `students` (menú Administración → Alumnos, permiso `FORM_STUDENT_MGMT`). El seed trae 15 filas (`ids` 1–15, nombres `juan` a `enzo`).

## Cursos de prueba (Ago-Dic 2026, seed de `05`)

| Curso | Aula | Día | Horario | Docentes | Alumnos |
|-------|------|-----|---------|----------|---------|
| Ingles A1 | Aula 101 | Lunes | 08:00-10:00 | brooks, omh | 5 |
| Ingles B1 | Aula 102 | Miércoles | 10:00-12:00 | conway | 3 |
| Frances A1 | Aula 201 | Martes | 14:00-16:00 | pareto | 4 |
| Portugues A1 | Laboratorio 1 | Jueves | 18:00-20:00 | amdahl, kirchhoff | 3 |
| Italiano A1 | Aula 101 | Viernes | 09:00-11:00 | murphy | 2 |
