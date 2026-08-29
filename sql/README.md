# Scripts SQL (`sql/`)

Esta carpeta contiene los scripts para crear, poblar y consultar la base de datos **`Trabajo_Final`** (SQL Server), junto con un respaldo de la base.

## Contenido

| Archivo | Descripción |
|---------|-------------|
| [`00_PurgeDatabase.sql`](00_PurgeDatabase.sql) | Elimina la base `Trabajo_Final` si ya existe (reinicialización limpia). |
| [`01_CreateTables.sql`](01_CreateTables.sql) | Crea el esquema completo: tablas, claves, relaciones y la jerarquía de roles/permisos. Incluye la tabla `ActivityLogs` del **Historial de Actividad**. |
| [`02_SeedData.sql`](02_SeedData.sql) | Carga los datos iniciales: roles, permisos y usuarios de prueba. |
| [`03_Queries.sql`](03_Queries.sql) | Consultas de ejemplo y verificación del funcionamiento. |
| [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) | Respaldo de la base de datos (para restaurarla directamente). |

## Cómo ejecutarla

Ejecutar los scripts **en orden** (1 al 3) sobre una instancia de SQL Server:

```sql
-- 1. Reinicializar
00_PurgeDatabase.sql
-- 2. Crear esquema
01_CreateTables.sql
-- 3. Cargar datos de prueba
02_SeedData.sql
-- (opcional) consultas de verificación
03_Queries.sql
```

La cadena de conexión se configura en el proyecto **GUI** (archivo `App.config`, clave `cadenaConexion`).

> Si la base ya existe y solo se quiere agregar el Historial de Actividad, ejecutar únicamente el `CREATE TABLE [dbo].[ActivityLogs]` de `01_CreateTables.sql`.
>
> Alternativamente, restaurar el respaldo [`Trabajo_Final_BBDD.bak`](Trabajo_Final_BBDD.bak) reemplaza todo el proceso.

## Datos de prueba

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `123` | Admin (todos los permisos) |
| `dario` | `123` | Admin (todos los permisos) |
| `pepe`  | `123` | Alumno (solo quejas) |
