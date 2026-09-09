# Proyecto Final

Repositorio de la materia **Proyecto Final** de la carrera **Analista Programador a Distancia**, Facultad de Tecnología Informática, Universidad Abierta Interamericana (UAI).

## Contenido del repositorio

Este repositorio agrupa **todas las partes** que conforman el trabajo final. A continuación se describe cada una y dónde encontrarla.

| Parte | Ubicación | Descripción |
|-------|-----------|-------------|
| **Aplicación .NET** | [`src/TrabajoFinal-DarioZubaray/`](src/TrabajoFinal-DarioZubaray/) | Solución de escritorio (WinForms, **.NET 10**, `net10.0-windows`) con arquitectura en capas. Incluye su propio README. |
| **Aplicación .NET — README** | [`src/TrabajoFinal-DarioZubaray/README.md`](src/TrabajoFinal-DarioZubaray/README.md) | Documentación del proyecto .NET: clases, capas, responsabilidades, patrones (Composite, Singleton/Multiton, Decorator, Strategy) y principios SOLID. |
| **Base de datos — scripts SQL** | [`src/sql/`](src/sql/) | Scripts ordenados para crear, poblar y consultar la base de datos `Trabajo_Final`. Ver [`src/sql/README.md`](src/sql/README.md). |
| **Diagramas (Mermaid Live)** | [`docs/mermaid-live/`](docs/mermaid-live/) | Diagramas en formato Mermaid: casos de uso, clases, ER y secuencias de login/logout. |
| **Modelos Enterprise Architect** | [`docs/ea/`](docs/ea/) | Archivos de modelado UML creados con Enterprise Architect. |
| **Documentación de la materia (MDS2)** | [`docs/MDS2/`](docs/MDS2/) | Examen final de la cursada Metodologías de Desarrollo 2. |
| **Documentación general (`docs/`)** | [`docs/README.md`](docs/README.md) | Guía de navegación del material de documentación (diagramas, modelos EA y cursada). |

### Aplicación .NET (`src/TrabajoFinal-DarioZubaray/`)

Solución con la siguiente arquitectura en capas (cada capa es un proyecto):

- **GUI** — interfaz gráfica (WinForms).
- **BLL** — lógica de negocio y servicios.
- **DAL** — acceso a datos de bajo nivel (SQL Server).
- **MPP** — capa de **mapeo**: transforma los objetos de base de datos (`DataTable`) en entidades de **BE** (y viceversa).
- **BE** — *Business Entities*: entidades, DTOs y el patrón **Composite** de roles/permisos.

Se destacan **cuatro** patrones de diseño:

- **Composite** — `IRoleComponentBE`, `RoleCompositeBE` y `PermissionLeafBE` representan la jerarquía de roles/permisos como un árbol.
- **Singleton / Multiton** — `ServiceLocatorBLL` (singleton de servicios) y `SessionManagerBLL` (multiton de sesiones, una por usuario).
- **Decorator** — `ActivityLoggingDecorator` envuelve una actividad (`IActivity`/`BaseActivity`) y, al finalizar, guarda un registro en el **Historial de Actividad**.
- **Strategy** — `PasswordHasher` elige entre algoritmos de hash intercambiables (`BcryptPasswordStrategy` y `LegacySha256PasswordStrategy`) para el cifrado/verificación de contraseñas.

Además, la aplicación aplica los **principios SOLID** (SRP, Open/Closed, Liskov, Interface Segregation y Dependency Inversion) — los patrones anteriores los materializan (p. ej. **Strategy** hace Open/Closed y facilita la inyección de dependencias) — todo documentado en el README del proyecto.

Para el detalle completo de clases, capas, responsabilidades y patrones, ver [`src/TrabajoFinal-DarioZubaray/README.md`](src/TrabajoFinal-DarioZubaray/README.md).

La aplicación también centraliza un **catálogo de códigos de error** por dominio (`ErrorCodesBLL`, prefijos `DB-`, `AUTH-`, `VAL-`, `BIZ-`, `GEN-`) con un **handler global** de excepciones en `Program.cs`. Ver la sección *"Manejo de errores y códigos de error"* en el README del proyecto .NET.

### Base de datos (`src/sql/`)

Contiene los scripts para crear, poblar y consultar la base `Trabajo_Final`, más un respaldo. Detalle completo en [`src/sql/README.md`](src/sql/README.md).

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

### Diagramas (`docs/mermaid-live/`)

Son archivos [Mermaid](https://mermaid.js.org/) que pueden visualizarse en GitHub o con herramientas que soporten el formato, como el [Mermaid Live Editor](https://mermaid.live/) (índice completo en [`docs/README.md`](docs/README.md)):

- `casos-uso.mmd` — casos de uso por actor (Admin, Profesor, Alumno).
- `clases.mmd` — diagrama de clases con el patrón Composite.
- `er.mmd` — modelo entidad-relación de la base.
- `secuencia-login.mmd` — secuencia simple del inicio de sesión (vista de alto nivel).
- `secuencia-login-completo.mmd` — secuencia completa con toda la cadena: AuthBLL → UserMPP → AccessDAL → SQL Server, ActivityBLL, PermissionBLL → RoleMPP, SessionManagerBLL, AppPreferencesBLL.
- `secuencia-logout.mmd` — secuencia simple del cierre de sesión (vista de alto nivel).
- `secuencia-logout-completo.mmd` — secuencia completa: AppPreferencesBLL (archivo), SessionManagerBLL (memoria), ActivityBLL → ActivityMPP → AccessDAL → SQL Server, CultureHelperBLL, ThemeHelper.

## Enlaces útiles

- [Página de la carrera](https://uai.edu.ar/facultades/tecnolog%C3%ADa-inform%C3%A1tica/analista-programador-a-distancia/)
