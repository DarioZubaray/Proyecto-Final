# TrabajoFinal-DarioZubaray (.NET)

Aplicación de escritorio **WinForms** sobre **.NET Framework 4.7.2** con arquitectura en **capas**. Es la parte central del trabajo final de la materia *Proyecto Final*.

## Arquitectura en capas

La solución está estructurada como una arquitectura en capas clásica. Cada capa es un proyecto separado y las dependencias van siempre "hacia abajo" (una capa conoce a las que están debajo, nunca al revés).

```
┌─────────────────────────────────────────────┐
│  GUI  (Interfaz gráfica - WinForms)         │  -> BE, BLL
├─────────────────────────────────────────────┤
│  BLL  (Lógica de negocio y servicios)       │  -> BE, MPP, DAL(indirecto)
├─────────────────────────────────────────────┤
│  MPP  (Persistencia: Modelo de Procedimientos)│ -> BE, DAL
├─────────────────────────────────────────────┤
│  DAL  (Acceso a datos - SQL Server)         │
├─────────────────────────────────────────────┤
│  BE   (Entidades de negocio)                │  (no depende de nada)
└─────────────────────────────────────────────┘
```

### Capas y proyectos

| Proyecto | Responsabilidad |
|----------|-----------------|
| **BE** (*Business Entities*) | Entidades del dominio: `UserBE`, `RoleBE`, `PermissionBE`. Contiene además el **DTO** `LoginResultBE` y la implementación del patrón **Composite** de roles. No depende de ninguna otra capa. |
| **DAL** (*Data Access Layer*) | Acceso a datos de bajo nivel. `AccessDAL` encapsula la conexión y ejecución de sentencias SQL (lectura, escalar y guardado) contra **SQL Server**, usando `SqlConnection`/`SqlCommand` y parámetros. |
| **MPP** (*Modelo de Persistencia de Procedimientos*) | Persistencia de las entidades sobre la base realizando los *mapeos* entre filas (`DataRow`) y entidades. `UserMPP` y `RoleMPP` implementan interfaces (`IUserMPP`, `IRoleMPP`), lo que permite inyectar mocks en pruebas. |
| **BLL** (*Business Logic Layer*) | Lógica y reglas de negocio. Se organiza en **Servicios** (`AuthBLL`, `UserBLL`, `RoleBLL`, `PermissionBLL`), **Helpers** (`EncryptionBLL`, `CultureHelperBLL`, `SessionManagerBLL`) y **ServiceLocatorBLL** (localizador/singleton de servicios). |
| **GUI** | Interfaz gráfica en WinForms: formularios de login, principal, usuarios, roles, preferencias y cambio de contraseña. El punto de entrada es `Program.cs`. |
| **BLL.Tests** / **MPP.Tests** | Proyectos de prueba unitaria (MSTest + Moq). Cubren la lógica de `AuthBLL`, `EncryptionBLL`, `CultureHelperBLL`, `UserBLL` y la persistencia de `UserMPP`. |

#### Dependencias entre proyectos

- **GUI** → referencia a `BE` y `BLL`.
- **BLL** → referencia a `BE` y `MPP`.
- **MPP** → referencia a `BE` y `DAL`.
- **BE** → sin dependencias.
- **BLL.Tests** → `BLL`, `BE`, `MPP` (+ `Moq`). **MPP.Tests** → `MPP`, `BE`, `DAL`.

## Descripción de clases por capa

### BE — Business Entities

| Clase | Responsabilidad |
|-------|-----------------|
| `UserBE` | Entidad *Usuario*. Modela datos de cuenta: nombre de usuario, hash de contraseña, estado activo, reintentos, fechas, idioma y rol asignado. |
| `RoleBE` | Entidad *Rol*. Posee la lista de permisos y el método `ToComposite(...)` que convierte el rol (y sus roles hijos) en un árbol **Composite**. |
| `PermissionBE` | Entidad *Permiso*. Identifica un formulario/acción concreta (`Name`, `Label`, `Description`, `IsSystem`). `IsSystem` marca permisos que no pueden quitarse. |
| `LoginResultBE` (DTO) | Resultado del inicio de sesión: `Success`, `Message` y `User` autenticado. |
| `IRoleComponentBE` | Interfaz común del **Composite** (componente). Define `Name`, `HasPermission(name)` y `GetAllPermissions()`. |
| `RoleCompositeBE` | Nodo **compuesto** del árbol: representa un rol que contiene hijos (otros roles y hojas de permiso). Permite agregar/quitar hijos y recorre el árbol para resolver permisos. |
| `PermissionLeafBE` | **Hoja** del árbol: envuelve un `PermissionBE` concreto. Evalúa `HasPermission` contra su propio nombre. |

### DAL — Data Access Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `AccessDAL` | Abstracción del acceso a SQL Server. Lee la cadena de conexión `cadenaConexion` de la configuración y ofrece `Read`, `ReadScalar` y `Save` (con parámetros opcionales). |

### MPP — Modelo de Persistencia de Procedimientos

| Clase | Responsabilidad |
|-------|-----------------|
| `IUserMPP` / `UserMPP` | Persistencia de usuarios. Mapea `DataRow ⇄ UserBE` y ejecuta operaciones de login (último acceso, reintentos, desactivación), CRUD, idioma y contraseña. |
| `IRoleMPP` / `RoleMPP` | Persistencia de roles y permisos. CRUD de roles, permisos por rol, jerarquía padre-hijo (`RoleHierarchy`) y asignación de permisos. |

### BLL — Business Logic Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `IAuthBLL` / `AuthBLL` | Autenticación. Valida credenciales, maneja hasta **3 reintentos**, desactiva el usuario superado el máximo y registra el último acceso. |
| `IUserBLL` / `UserBLL` | Lógica de gestión de usuarios: CRUD, búsqueda, cambio de idioma y cambio de contraseña (verificando la actual). |
| `IRoleBLL` / `RoleBLL` | Lógica de gestión de roles: CRUD, permisos por rol (protegiendo los de sistema) y jerarquía de roles. |
| `PermissionBLL` | Construye el árbol **Composite** de un rol mediante `BuildRoleTree(roleId)` y consulta permisos sobre el árbol (`HasPermission`). |
| `EncryptionBLL` (static) | Hash y verificación de contraseñas con **BCrypt**; además verifica hashes "legacy" (SHA-256 de 64 hex) para compatibilidad con datos existentes. |
| `CultureHelperBLL` (static) | Gestión de idioma/cultura (`es`, `en`, `pt-BR`). Define los idiomas soportados y aplica la cultura al hilo actual. |
| `ServiceLocatorBLL` (static) | **Singleton / Localizador de servicios.** Mantiene una única instancia por servicio de persistencia y crea la lógica de negocio correspondiente. |
| `SessionManagerBLL` | **Multiton de sesiones.** Mantiene el estado de la sesión por usuario (multiton: una instancia por `userId`). |

### GUI — Interfaz gráfica

| Formulario | Responsabilidad |
|------------|-----------------|
| `LoginForm` | Autenticación. Crea la sesión (`SessionManagerBLL.CreateSession`), abre el menú principal y la limpia al cerrar (`RemoveSession`). |
| `MainForm` | Menú principal (MDI). Muestra el usuario/rol en el pie y **oculta o muestra opciones según los permisos** del árbol Composite de la sesión. |
| `UserManagementForm` / `UserForm` | ABM de usuarios: listado, búsqueda, alta, edición y baja lógica. |
| `RoleManagementForm` | ABM de roles y asignación de permisos (manteniendo los de sistema). |
| `PreferencesForm` | Cambio de idioma; actualiza la sesión y refresca los recursos de la UI. |
| `ChangePasswordForm` | Cambio de contraseña validando la actual. |
| `TestComplaintsForm` / `TestReportsForm` | Formularios de ejemplo (quejas/reportes) usados por los permisos `FORM_COMPLAINTS` y `FORM_REPORTS`. |

## Patrones de diseño destacados

### 1) Composite — jerarquía de roles y permisos

El patrón **Composite** permite tratar por igual a un rol completo y a un permiso individual, de modo que los permisos de un usuario se modelan como un **árbol**.

```
IRoleComponentBE  (interfaz componente)
   ├─ RoleCompositeBE  (compuesto: contiene hijos)
   └─ PermissionLeafBE (hoja: envuelve un PermissionBE)
```

Participantes:

- **Componente** → `IRoleComponentBE` — `Name`, `HasPermission(name)`, `GetAllPermissions()`.
- **Compuesto** → `RoleCompositeBE` — mantiene una lista `_children` y dispone de `AddChild`/`RemoveChild`. `HasPermission` delega en cada hijo y `GetAllPermissions` concatena los resultados.
- **Hoja** → `PermissionLeafBE` — envuelve un `PermissionBE` concreto; `HasPermission` compara su propio nombre.

**Construcción del árbol**:
1. `SessionManagerBLL.CreateSession(user)` invoca `PermissionBLL.BuildRoleTree(user.RoleId)`.
2. `RoleMPP.FindById(roleId)` obtiene el rol con sus permisos; `GetChildRoleIds` obtiene los roles hijos.
3. `RoleBE.ToComposite(childRoles)` crea un `RoleCompositeBE` al que le agrega una hoja `PermissionLeafBE` por cada permiso y los roles hijos (recursivamente).

**Uso**: `MainForm` consulta `_session.HasPermission("FORM_USER_MGMT")` para decidir si muestra el menú de administración. El propio `SessionManagerBLL.HasPermission` delega en el árbol.

### 2) Singleton (Service Locator) — `ServiceLocatorBLL`

`ServiceLocatorBLL` es una **clase estática (singleton)** que centraliza la creación de dependencias de la capa de negocio y evita crear instancias duplicadas de los acumuladores de persistencia.

```csharp
public static class ServiceLocatorBLL
{
    private static IUserMPP _userMPP;   // única instancia
    private static IRoleMPP _roleMPP;   // única instancia

    public static IUserMPP GetUserMPP()
    {
        if (_userMPP == null) _userMPP = new UserMPP();  // lazy singleton
        return _userMPP;
    }
    // ... CreateAuthBLL(), CreateUserBLL(), CreateRoleBLL(), etc.
}
```

- Garantiza **una sola instancia** de cada servicio de persistencia durante todo el ciclo de vida de la aplicación (*singleton*).
- **Descarga la responsabilidad** de instanciación de las formas de la GUI: `LoginForm`, `UserForm`, `UserManagementForm`, `RoleManagementForm`, `ChangePasswordForm` y `PreferencesForm` obtienen sus servicios a través de `ServiceLocatorBLL`.

### 3) Multiton — `SessionManagerBLL`

Un **Multiton** es una variante del Singleton: en lugar de una única instancia global, mantiene **una instancia por clave** (aquí, una por `userId`).

```csharp
public class SessionManagerBLL
{
    private static Dictionary<int, SessionManagerBLL> _instances;  // una por userId

    private SessionManagerBLL(UserBE user, RoleCompositeBE roleTree) { ... }  // ctor privado

    public static SessionManagerBLL GetInstance(int userId) { ... }
    public static SessionManagerBLL CreateSession(UserBE user) { ... }
    public static void RemoveSession(int userId) { ... }
}
```

- El **constructor es privado**: solo se crea una sesión mediante `CreateSession(user)`, que la guarda en `_instances[user.Id]`.
- `GetInstance(userId)` recupera la sesión **existente** de un usuario (devuelve `null` si no la hay).
- `RemoveSession(userId)` la elimina al cerrar sesión.
- Cada sesión conserva el **usuario** y el **árbol Composite de su rol**, permitiendo consultas de permisos (`HasPermission`) y cambios de idioma.
- Al iniciar sesión, `LoginForm` llama a `CreateSession(result.User)`; `MainForm` y `PreferencesForm` recuperan la sesión con `GetInstance(user.Id)`.

## Cómo ejecutar

1. Abrir la solución `TrabajoFinal-DarioZubaray.sln` (Visual Studio).
2. Configurar la cadena de conexión `cadenaConexion` (proyecto **GUI**, archivo `App.config`) apuntando a una instancia de SQL Server con la base `Trabajo_Final`.
3. Ejecutar los scripts de [`sql/`](../../sql/) en orden (véase el README global).
4. Compilar y ejecutar. Usuarios de prueba: `admin` / `123` (rol Admin), `pepe` / `123` (rol Alumno).

## Pruebas

Los proyectos **BLL.Tests** y **MPP.Tests** (MSTest + Moq) cubren la autenticación, el cifrado de contraseñas, la gestión de idioma, la lógica de usuarios y la persistencia. Para ejecutarlos:

```
dotnet test TrabajoFinal-DarioZubaray.sln
```
