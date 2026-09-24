# Scripts SQL (`src/sql/`)

Scripts para crear, poblar y consultar la base de datos **`DarioZubaray_TF`** (SQL Server).

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [`00_v0.0.1_PurgeDatabase.sql`](00_v0.0.1_PurgeDatabase.sql) | Elimina todas las tablas (reinicialización limpia). |
| [`01_v1.0.0_CreateTables.sql`](01_v1.0.0_CreateTables.sql) | Esquema base: Roles, Permissions, RolePermissions, RoleHierarchy, Users, ActivityLogs, SchemaVersions. |
| [`02_v1.0.0_SeedData.sql`](02_v1.0.0_SeedData.sql) | Datos iniciales: roles, permisos, 24 usuarios de prueba. |
| [`03_v1.1.0_AulasCursosInscripciones.sql`](03_v1.1.0_AulasCursosInscripciones.sql) | Migración consolidada: classrooms, courses, course_teachers, course_students, attendance. Permisos, rol Coordinador. Seed data completo. |
| [`04_v1.1.0_Rollback.sql`](04_v1.1.0_Rollback.sql) | Rollback de v1.1.0. |
| [`05_v1.1.0_Queries.sql`](05_v1.1.0_Queries.sql) | Consultas de verificación y desarrollo. |
| [`06_v1.1.0_ScriptConsolidado.sql`](06_v1.1.0_ScriptConsolidado.sql) | **Script maestro**: crea la base (si no existe), crea todas las tablas, inserta los datos y registra las versiones en un solo paso. |

## Instalación

### Opción A — Script maestro (recomendada, un solo paso)

Ejecutar únicamente [`06_v1.1.0_ScriptConsolidado.sql`](06_v1.1.0_ScriptConsolidado.sql). Crea la base `DarioZubaray_TF` si no existe, dropea las tablas existentes, crea el esquema completo (v1.0.0 + v1.1.0) y carga todos los datos.

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

-- 5. (opcional) Consultas de verificación
05_v1.1.0_Queries.sql
```

## Convención de nombres

**Tablas v1.0.0** (PascalCase, se mantienen por compatibilidad):
`Roles`, `Permissions`, `RolePermissions`, `RoleHierarchy`, `Users`, `ActivityLogs`, `SchemaVersions`

**Tablas v1.1.0+** (snake_case plural inglés):
`classrooms`, `courses`, `course_teachers`, `course_students`, `attendance`

**Columnas**: Todas `snake_case` (`start_date`, `day_of_week`, `is_active`, etc.)

## Usuarios de prueba

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `123` | Admin |
| `dario` | `123` | Admin |
| `coord_maria`, `coord_carlos`, `coord_laura` | `123` | Coordinador |
| `prof_garcia` a `prof_fernandez` (5) | `123` | Profesor |
| `alumno_perez` a `alumno_soto` (15) | `123` | Alumno |

## Cursos de prueba (Ago-Dic 2026)

| Curso | Aula | Día | Horario | Docentes | Alumnos |
|-------|------|-----|---------|----------|---------|
| Ingles Basico A1 | Aula 101 | Lunes | 08:00-10:00 | garcia, lopez | 5 |
| Ingles Intermedio B1 | Aula 102 | Miércoles | 10:00-12:00 | martinez | 3 |
| Frances Inicial A1 | Aula 201 | Martes | 14:00-16:00 | rodriguez | 4 |
| Portugues Basico | Lab 1 | Jueves | 18:00-20:00 | fernandez, garcia | 3 |
| Italiano A1 | Aula 101 | Viernes | 09:00-11:00 | lopez | 2 |
