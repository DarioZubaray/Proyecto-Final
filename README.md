# Proyecto Final

Repositorio de la materia **Proyecto Final** de la carrera **Analista Programador a Distancia**, Facultad de Tecnología Informática, Universidad Abierta Interamericana (UAI).

## Contenido del repositorio

Este repositorio agrupa **todas las partes** que conforman el trabajo final. A continuación se describe cada una y dónde encontrarla.

| Parte | Ubicación | Descripción |
|-------|-----------|-------------|
| **Aplicación .NET** | [`src/TrabajoFinal-DarioZubaray/`](src/TrabajoFinal-DarioZubaray/) | Solución de escritorio (WinForms, .NET Framework 4.7.2) con arquitectura en capas. Incluye su propio README. |
| **Aplicación .NET — README** | [`src/TrabajoFinal-DarioZubaray/README.md`](src/TrabajoFinal-DarioZubaray/README.md) | Documentación del proyecto .NET: clases, capas, responsabilidades y patrones (Composite, Singleton/Multiton). |
| **Base de datos — scripts SQL** | [`sql/`](sql/) | Scripts ordenados para crear, poblar y consultar la base de datos `Trabajo_Final`. |
| **Diagramas** | [`diagramas/`](diagramas/) | Diagramas en formato Mermaid: casos de uso, clases, ER y secuencia de login. |
| **Modelo UML / Enterprise Architect** | [`ea/`](ea/) | Proyecto modelo de Enterprise Architect (`.eapx`). |
| **Documentación de la materia (MDS2)** | [`docs/MDS2/`](docs/MDS2/) | Examen final de la cursada. |

### Aplicación .NET (`src/TrabajoFinal-DarioZubaray/`)

Solución con la siguiente arquitectura en capas (cada capa es un proyecto):

- **GUI** — interfaz gráfica (WinForms).
- **BLL** — lógica de negocio y servicios.
- **DAL** — acceso a datos de bajo nivel (SQL Server).
- **MPP** — *Modelo de Persistencia de Procedimientos*: persiste las entidades del modelo sobre la base.
- **BE** — *Business Entities*: entidades, DTOs y el patrón **Composite** de roles/permisos.

Se destacan dos patrones de diseño:

- **Composite** — `IRoleComponentBE`, `RoleCompositeBE` y `PermissionLeafBE` representan la jerarquía de roles/permisos como un árbol.
- **Singleton / Multiton** — `ServiceLocatorBLL` (singleton de servicios) y `SessionManagerBLL` (multiton de sesiones, una por usuario).

📖 Para el detalle completo de clases, capas, responsabilidades y patrones, ver [`src/TrabajoFinal-DarioZubaray/README.md`](src/TrabajoFinal-DarioZubaray/README.md).

### Base de datos (`sql/`)

Ejecutar los scripts en orden:

1. `00_PurgeDatabase.sql` — elimina la base si existe (reinicialización).
2. `01_CreateTables.sql` — crea el esquema (tablas, claves, relaciones y jerarquía de roles).
3. `02_SeedData.sql` — carga datos iniciales (roles, permisos y usuarios de prueba).
4. `03_Queries.sql` — consultas de ejemplo / verificación.

#### Datos de prueba

| Usuario | Contraseña | Rol |
|---------|-----------|-----|
| `admin` | `123` | Admin (todos los permisos) |
| `dario` | `123` | Admin (todos los permisos) |
| `pepe`  | `123` | Alumno (solo quejas) |

### Diagramas (`diagramas/`)

Son archivos [Mermaid](https://mermaid.js.org/) que pueden visualizarse en GitHub o con herramientas que soporten el formato:

- `casos-uso.mmd` — casos de uso por actor (Admin, Supervisor, Operador).
- `clases.mmd` — diagrama de clases con el patrón Composite.
- `er.mmd` — modelo entidad-relación de la base.
- `secuencia-login.mmd` — secuencia del inicio de sesión (usa Singleton/Multiton y Composite).

## Enlaces útiles

- [Página de la carrera](https://uai.edu.ar/facultades/tecnolog%C3%ADa-inform%C3%A1tica/analista-programador-a-distancia/)
