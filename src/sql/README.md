# Scripts SQL (`src/sql/`)

Esta carpeta contiene los scripts para crear, poblar y consultar la base de datos **`Trabajo_Final`** (SQL Server), junto con un respaldo de la base.

## Contenido

### Esquema base (v1.0.0)

| Archivo | Descripción |
|---------|-------------|
| [`00_v1.0.0_PurgeDatabase.sql`](00_v1.0.0_PurgeDatabase.sql) | Elimina todas las tablas de la base `Trabajo_Final` (reinicialización limpia). |
| [`01_v1.0.0_CreateTables.sql`](01_v1.0.0_CreateTables.sql) | Crea el esquema completo: tablas, claves, relaciones, jerarquía de roles, ActivityLogs, SchemaVersions, Aulas, Cursos y CursoDocentes. |
| [`02_v1.0.0_SeedData.sql`](02_v1.0.0_SeedData.sql) | Carga datos iniciales: roles, permisos, usuarios de prueba, aulas y cursos de idiomas. |
| [`03_v1.0.0_Queries.sql`](03_v1.0.0_Queries.sql) | Consultas de ejemplo y verificación del funcionamiento. |

### Migraciones

| Archivo | Descripción |
|---------|-------------|
| [`04_v1.1.0_AulasCursos.sql`](04_v1.1.0_AulasCursos.sql) | Migración v1.1.0: Agrega tablas Aulas, Cursos, CursoDocentes, permiso FORM_CURSO_MGMT y rol Coordinador. Solo para bases que ya tienen el esquema v1.0.0. |

### Otros

| Archivo | Descripción |
|---------|-------------|
| [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) | Respaldo de la base de datos (para restaurarla directamente). |

## Cómo ejecutar

### Opción 1: Instalación limpia (recomendado)

Ejecutar los scripts **en orden** sobre una instancia de SQL Server:

```sql
-- 1. Reinicializar (eliminar tablas existentes)
00_v1.0.0_PurgeDatabase.sql

-- 2. Crear esquema completo (incluye Aulas, Cursos, etc.)
01_v1.0.0_CreateTables.sql

-- 3. Cargar datos de prueba (roles, usuarios, aulas, cursos)
02_v1.0.0_SeedData.sql

-- (opcional) 4. Consultas de verificación
03_v1.0.0_Queries.sql
```

### Opción 2: Migración desde v1.0.0

Si la base ya tiene el esquema v1.0.0 (sin Aulas/Cursos):

```sql
-- Ejecutar solo la migración
04_v1.1.0_AulasCursos.sql
```

### Opción 3: Restaurar respaldo

Restaurar el archivo [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) reemplaza todo el proceso.

La cadena de conexión se configura en el proyecto **GUI** (archivo `App.config`, clave `cadenaConexion`).

## Datos de prueba

### Usuarios

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `123` | Admin (todos los permisos) |
| `dario` | `123` | Admin (todos los permisos) |
| `coord_maria` | `123` | Coordinador (ABMs) |
| `coord_carlos` | `123` | Coordinador (ABMs) |
| `coord_laura` | `123` | Coordinador (ABMs) |
| `prof_garcia` | `123` | Profesor (cursos, quejas, reportes) |
| `prof_lopez` | `123` | Profesor (cursos, quejas, reportes) |
| `prof_martinez` | `123` | Profesor (cursos, quejas, reportes) |
| `prof_rodriguez` | `123` | Profesor (cursos, quejas, reportes) |
| `prof_fernandez` | `123` | Profesor (cursos, quejas, reportes) |
| `alumno_perez` a `alumno_soto` | `123` | Alumno (solo quejas) |

### Aulas

| Aula | Capacidad |
|------|-----------|
| Aula 101 - Goiescalza | 30 |
| Aula 102 - Goiescalza | 30 |
| Aula 201 - Goiescalza | 25 |
| Laboratorio 1 - Goiescalza | 20 |
| Aula Magna - Goiescalza | 100 |

### Cursos de idiomas (Ago-Dic 2026)

| Curso | Aula | Docentes |
|-------|------|----------|
| Ingles Basico A1 | Aula 101 | prof_garcia, prof_lopez |
| Ingles Intermedio B1 | Aula 102 | prof_martinez |
| Frances Inicial A1 | Aula 201 | prof_rodriguez |
| Portugues Basico | Laboratorio 1 | prof_fernandez, prof_garcia |
| Italiano A1 | Sin asignar | prof_lopez |
