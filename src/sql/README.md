# Scripts SQL (`src/sql/`)

Esta carpeta contiene los scripts para crear, poblar y consultar la base de datos **`Trabajo_Final`** (SQL Server), junto con un respaldo de la base.

## Contenido

### Esquema base (v1.0.0)

| Archivo | Descripción |
|---------|-------------|
| [`00_v0.0.1_PurgeDatabase.sql`](00_v0.0.1_PurgeDatabase.sql) | Elimina todas las tablas de la base `Trabajo_Final` (reinicialización limpia). Orden correcto respetando foreign keys. |
| [`01_v1.0.0_CreateTables.sql`](01_v1.0.0_CreateTables.sql) | Crea el esquema base: Roles, Permissions, RolePermissions, RoleHierarchy, Users, ActivityLogs, SchemaVersions. |
| [`02_v1.0.0_SeedData.sql`](02_v1.0.0_SeedData.sql) | Carga datos iniciales: roles, permisos, usuarios de prueba (solo login). |

### Migración v1.1.0 (consolidada)

| Archivo | Descripción |
|---------|-------------|
| [`03_v1.1.0_AulasCursosInscripciones.sql`](03_v1.1.0_AulasCursosInscripciones.sql) | Migración consolidada v1.1.0: Crea tablas Aulas, Cursos (con horarios), CursoDocentes, CursoAlumnos, ClasesAlumnos. Agrega permisos CURSO_MGMT, INSCRIPCION_MGMT, ASISTENCIA_MGMT, ASISTENCIA_VIEW. Rol Coordinador. Seed data de aulas, cursos, docentes, inscripciones y asistencia. |
| [`04_v1.1.0_Rollback.sql`](04_v1.1.0_Rollback.sql) | Rollback de v1.1.0: Elimina tablas creadas, permisos y rol Coordinador. |

### Consultas de desarrollo

| Archivo | Descripción |
|---------|-------------|
| [`05_v1.1.0_Queries.sql`](05_v1.1.0_Queries.sql) | Consultas de ejemplo y verificación: usuarios, roles, permisos, aulas, cursos, inscripciones, asistencia. |

### Otros

| Archivo | Descripción |
|---------|-------------|
| [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) | Respaldo de la base de datos (para restaurarla directamente). |

## Cómo ejecutar

### Opción 1: Instalación limpia (recomendado)

Ejecutar los scripts **en orden** sobre una instancia de SQL Server:

```sql
-- 1. Reinicializar (eliminar tablas existentes)
00_v0.0.1_PurgeDatabase.sql

-- 2. Crear esquema base (login, usuarios, roles)
01_v1.0.0_CreateTables.sql

-- 3. Cargar datos de prueba (roles, usuarios)
02_v1.0.0_SeedData.sql

-- 4. Aplicar migración v1.1.0 (aulas, cursos, inscripciones, asistencia)
03_v1.1.0_AulasCursosInscripciones.sql

-- (opcional) 5. Consultas de verificación
05_v1.1.0_Queries.sql
```

### Opción 2: Rollback de v1.1.0

Si se necesita deshacer la migración v1.1.0:

```sql
04_v1.1.0_Rollback.sql
```

### Opción 3: Restaurar respaldo

Restaurar el archivo [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) reemplaza todo el proceso.

La cadena de conexión se configura en el proyecto **GUI** (archivo `App.config`, clave `cadenaConexion`).

## Convención de nombres (snake_case)

Todas las tablas y columnas siguen el estándar `snake_case`:

| Tabla | Columnas |
|-------|----------|
| `Roles` | `id`, `name` |
| `Permissions` | `id`, `name`, `label`, `description`, `is_system` |
| `Users` | `id`, `user_name`, `password_hash`, `is_active`, `retries_count`, `last_update`, `created_at`, `role_id`, `language`, `theme` |
| `Aulas` | `id`, `nombre`, `capacidad`, `is_active`, `created_at`, `last_update` |
| `Cursos` | `id`, `nombre`, `descripcion`, `fecha_inicio`, `fecha_fin`, `aula_id`, `dia_semana`, `hora_inicio`, `hora_fin`, `is_active`, `created_at`, `last_update` |
| `CursoDocentes` | `id`, `curso_id`, `docente_id`, `is_active`, `created_at` |
| `CursoAlumnos` | `id`, `curso_id`, `alumno_id`, `is_active`, `created_at` |
| `ClasesAlumnos` | `id`, `curso_id`, `alumno_id`, `fecha`, `presente`, `created_at` |

## Datos de prueba

### Usuarios

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `123` | Admin (todos los permisos) |
| `dario` | `123` | Admin (todos los permisos) |
| `coord_maria` | `123` | Coordinador (ABMs) |
| `coord_carlos` | `123` | Coordinador (ABMs) |
| `coord_laura` | `123` | Coordinador (ABMs) |
| `prof_garcia` | `123` | Profesor (cursos, inscripciones, asistencia) |
| `prof_lopez` | `123` | Profesor (cursos, inscripciones, asistencia) |
| `prof_martinez` | `123` | Profesor (cursos, inscripciones, asistencia) |
| `prof_rodriguez` | `123` | Profesor (cursos, inscripciones, asistencia) |
| `prof_fernandez` | `123` | Profesor (cursos, inscripciones, asistencia) |
| `alumno_perez` a `alumno_soto` | `123` | Alumno (solo inscripciones) |

### Aulas

| Aula | Capacidad |
|------|-----------|
| Aula 101 - Goiescalza | 30 |
| Aula 102 - Goiescalza | 30 |
| Aula 201 - Goiescalza | 25 |
| Laboratorio 1 - Goiescalza | 20 |
| Aula Magna - Goiescalza | 100 |

### Cursos de idiomas (Ago-Dic 2026)

| Curso | Aula | Día | Horario | Docentes |
|-------|------|-----|---------|----------|
| Ingles Basico A1 | Aula 101 | Lunes | 08:00 - 10:00 | prof_garcia, prof_lopez |
| Ingles Intermedio B1 | Aula 102 | Miércoles | 10:00 - 12:00 | prof_martinez |
| Frances Inicial A1 | Aula 201 | Martes | 14:00 - 16:00 | prof_rodriguez |
| Portugues Basico | Laboratorio 1 | Jueves | 18:00 - 20:00 | prof_fernandez, prof_garcia |
| Italiano A1 | Aula 101 | Viernes | 09:00 - 11:00 | prof_lopez |

### Inscripciones (alumnos por curso)

| Curso | Alumnos inscriptos |
|-------|-------------------|
| Ingles Basico A1 | alumno_perez, alumno_gomez, alumno_diaz, alumno_morales, alumno_torres |
| Ingles Intermedio B1 | alumno_ramos, alumno_silva, alumno_castro |
| Frances Inicial A1 | alumno_ruiz, alumno_alvarez, alumno_romero, alumno_sanchez |
| Portugues Basico | alumno_herrera, alumno_patricio, alumno_soto |
| Italiano A1 | alumno_perez, alumno_ruiz |

### Asistencia (ejemplo: 2 semanas)

Se registran 25 registros de asistencia para las primeras 2 semanas de clase (ago 2026), incluyendo presentes y ausentes.
