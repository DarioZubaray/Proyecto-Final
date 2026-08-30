# TrabajoFinal-DarioZubaray (.NET)

Aplicación de escritorio **WinForms sobre .NET 10** (SDK 10.0.400+, plataforma `net10.0-windows`) con arquitectura en **capas**. Es la parte central del trabajo final de la materia *Proyecto Final*.

## Arquitectura en capas

La solución está estructurada como una arquitectura en capas clásica. Cada capa es un proyecto separado y las dependencias van siempre "hacia abajo" (una capa conoce a las que están debajo, nunca al revés).

```
┌───────────────────────────────────────────────┐
│  GUI  (Interfaz gráfica - WinForms)           │  -> BE, BLL
├───────────────────────────────────────────────┤
│  BLL  (Lógica de negocio y servicios)         │  -> BE, MPP, DAL(indirecto)
├───────────────────────────────────────────────┤
│  MPP  (Mapeador)                              │  -> BE, DAL
├───────────────────────────────────────────────┤
│  DAL  (Acceso a datos - SQL Server)           │
├───────────────────────────────────────────────┤
│  BE   (Entidades de negocio)                  │  (no depende de nada)
└───────────────────────────────────────────────┘
```

### Capas y proyectos

| Proyecto | Responsabilidad |
|----------|-----------------|
| **BE** (*Business Entities*) | Entidades del dominio: `UserBE`, `RoleBE`, `PermissionBE`. Contiene además el **DTO** `LoginResultBE` y la implementación del patrón **Composite** de roles. No depende de ninguna otra capa. |
| **DAL** (*Data Access Layer*) | Acceso a datos de bajo nivel. `AccessDAL` encapsula la conexión y ejecución de sentencias SQL (lectura, escalar y guardado) contra **SQL Server**, usando `SqlConnection`/`SqlCommand` y parámetros. |
| **MPP** (*Mapper*) | Capa de **mapeo** entre la base de datos y el modelo de negocio: solo transforma un `DataTable` (o similar) devuelto por `DAL` a un objeto de **BE**, y viceversa. `UserMPP`, `RoleMPP` y `ActivityMPP` implementan interfaces (`IUserMPP`, `IRoleMPP`, `IActivityMPP`), lo que permite inyectar mocks en pruebas. |
| **BLL** (*Business Logic Layer*) | Lógica y reglas de negocio. Se organiza en **Servicios** (`AuthBLL`, `UserBLL`, `RoleBLL`, `PermissionBLL`, `ActivityBLL`), **Helpers** (`EncryptionBLL`, `CultureHelperBLL`, `SessionManagerBLL`) y **ServiceLocatorBLL** (localizador/singleton de servicios). |
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
| `ActivityLogBE` | Entidad que representa un registro del **Historial de Actividad**: usuario, acción, formulario, detalle y fecha/hora. |
| `IRoleComponentBE` | Interfaz común del **Composite** (componente). Define `Name`, `HasPermission(name)` y `GetAllPermissions()`. |
| `RoleCompositeBE` | Nodo **compuesto** del árbol: representa un rol que contiene hijos (otros roles y hojas de permiso). Permite agregar/quitar hijos y recorre el árbol para resolver permisos. |
| `PermissionLeafBE` | **Hoja** del árbol: envuelve un `PermissionBE` concreto. Evalúa `HasPermission` contra su propio nombre. |

### DAL — Data Access Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `AccessDAL` | Abstracción del acceso a SQL Server. Lee la cadena de conexión `cadenaConexion` de la configuración y ofrece `Read`, `ReadScalar` y `Save` (con parámetros opcionales). |

### MPP — Modelo de Procedimientos de Persistencia (capa de mapeo)

| Clase | Responsabilidad |
|-------|-----------------|
| `IUserMPP` / `UserMPP` | Mapeo de usuarios: transforma filas (`DataRow`) en `UserBE` y viceversa, y persiste las operaciones de login (último acceso, reintentos, desactivación), CRUD, idioma y contraseña. |
| `IRoleMPP` / `RoleMPP` | Mapeo de roles y permisos: transforma filas en `RoleBE`/`PermissionBE`, CRUD de roles, permisos por rol, jerarquía padre-hijo (`RoleHierarchy`) y asignación de permisos. |
| `IActivityMPP` / `ActivityMPP` | Mapeo del historial de actividad: transforma filas en `ActivityLogBE`, inserta registros y consulta **paginada** (con `OFFSET`/`FETCH`) filtrando por usuario. |

### BLL — Business Logic Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `IAuthBLL` / `AuthBLL` | Autenticación. Valida credenciales, maneja hasta **3 reintentos**, desactiva el usuario superado el máximo y registra el último acceso. |
| `IUserBLL` / `UserBLL` | Lógica de gestión de usuarios: CRUD, búsqueda, cambio de idioma y cambio de contraseña (verificando la actual). |
| `IRoleBLL` / `RoleBLL` | Lógica de gestión de roles: CRUD, permisos por rol (protegiendo los de sistema) y jerarquía de roles. |
| `PermissionBLL` | Construye el árbol **Composite** de un rol mediante `BuildRoleTree(roleId)` y consulta permisos sobre el árbol (`HasPermission`). |
| `IActivityBLL` / `ActivityBLL` | Servicio del **Historial de Actividad**. Registra accesos a formularios y el inicio/cierre de sesión (usando el **Decorator**) y consulta el historial de forma paginada. |
| `IActivity` (componente) | Interfaz componente del patrón **Decorator**: define `Execute()` y los datos de una actividad. |
| `BaseActivity` | Componente concreto del **Decorator**: representa una actividad concreta (acceso a formulario, login o logout) sin efectos secundarios. |
| `ActivityLoggingDecorator` | Decorador del **Decorator**: envuelve una `IActivity` y, al finalizar, guarda el registro en la base. |
| `EncryptionBLL` (static) | Fachada de compatibilidad del hash/verificación de contraseñas: delega en `PasswordHasher.Default` (patrón **Strategy**). |
| `IPasswordStrategy` | Interfaz estrategia del **Strategy**: `Matches`, `Hash` y `Verify`. |
| `BcryptPasswordStrategy` | Estrategia concreta que cifra y verifica con **BCrypt** (algoritmo actual). |
| `LegacySha256PasswordStrategy` | Estrategia concreta de **SHA-256** (64 hex) para verificar contraseñas legadas de datos existentes. |
| `PasswordHasher` | Contexto del **Strategy**: elige la estrategia adecuada según el formato del hash y expone `Hash`/`Verify`. |
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
| `ActivityHistoryForm` | **Historial de Actividad** (dentro del menú Archivo): lista paginada de las actividades del usuario autenticado. |
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

### 4) Decorator — Historial de Actividad

El **Decorator** permite añadir responsabilidades a un objeto sin modificar su clase, envolviéndolo en otro objeto que implementa la misma interfaz. Se usa para registrar en el **Historial de Actividad**: se envuelve la "actividad" que se quiere realizar y, al finalizar, se guarda el registro en la base.

```
IActivity  (interfaz componente: Execute())
   ├─ BaseActivity            (componente concreto: actividad de acceso formulario / login / logout)
   └─ ActivityLoggingDecorator (decorator: ejecuta la actividad y luego guarda en la base)
```

Participantes:

- **Componente** → `IActivity` — `UserId`, `Action`, `FormName`, `Description` y `Execute()`.
- **Componente concreto** → `BaseActivity` — una actividad concreta; `Execute()` no produce efectos secundarios.
- **Decorator** → `ActivityLoggingDecorator` — recibe una `IActivity` y un `IActivityMPP`; en `Execute()` primero ejecuta la actividad envuelta y luego **guarda el registro** (`_activityMPP.Save(...)`).

**Uso**: `ActivityBLL` expone `LogFormAccess`, `LogLogin` y `LogLogout`, que construyen `new ActivityLoggingDecorator(new BaseActivity(...), _activityMPP).Execute()`. Los puntos de registro son:

- **LoginForm**: tras autenticar (`LogLogin`) y al cerrar sesión (`LogLogout`).
- **MainForm**: cada opción del menú que abre un formulario registra un `LogFormAccess` (Preferencias, Usuarios, Roles, Cambiar Contraseña).
- **ActivityHistoryForm**: lista paginada de las actividades del usuario autenticado (no se registra a sí misma, para evitar ruido).

### 5) Strategy — algoritmos de hash de contraseñas

El **Strategy** permite intercambiar algoritmos en tiempo de ejecución sin modificar a quien los consume. Se usa para la verificación de contraseñas, que históricamente mezclaba dos algoritmos en un único `if/else` y hoy delega en estrategias intercambiables.

```
IPasswordStrategy  (interfaz estrategia: Matches / Hash / Verify)
   ├─ BcryptPasswordStrategy        (BCrypt)
   └─ LegacySha256PasswordStrategy  (SHA-256 legacy, para datos existentes)
PasswordHasher  (contexto: elige la estrategia según el formato del hash)
```

Participantes:

- **Estrategia** → `IPasswordStrategy` — `Matches(storedHash)` (dice si el hash le corresponde), `Hash(password)` y `Verify(plain, stored)`.
- **Estrategia concreta** → `BcryptPasswordStrategy` (BCrypt) y `LegacySha256PasswordStrategy` (SHA-256 de 64 hex), esta última para mantener la compatibilidad con usuarios creados antes del cifrado BCrypt.
- **Contexto** → `PasswordHasher` — mantiene una lista ordenada de estrategias; `Verify` recorre las estrategias, delega en la que `Matches` el hash guardado y, si ninguna, usa la estrategia por defecto (BCrypt).

**Uso**: `EncryptionBLL` (podado a ser una fachada de compatibilidad) delega en `PasswordHasher.Default` para `HashPassword` y `VerifyPassword`; `AuthBLL`, `UserBLL` y `GUI/UserForm` siguen usando la misma API sin cambios. Agregar un tercer algoritmo (p. ej. Argon2) solo implica una nueva `IPasswordStrategy`: **Open/Closed y sin tocar el contexto**.

## Principios SOLID aplicados

El código aplica los principios SOLID, en gran medida de la mano de los patrones ya documentados.

### S — Single Responsibility (Responsabilidad Única)

Cada tipo tiene **un único motivo de cambio**:

- **Capas** con responsabilidad única: BE (entidades), DAL (acceso a datos), MPP (mapeo), BLL (lógica de negocio) y GUI (presentación).
- En **BLL**, una clase por responsabilidad: `AuthBLL` (autenticación), `UserBLL` (usuarios), `RoleBLL` (roles), `PermissionBLL` (árbol de permisos) y `ActivityBLL` (historial de actividad). Helpers como `EncryptionBLL` (cifrado), `CultureHelperBLL` (idioma) y `SessionManagerBLL` (sesión) hacen una sola cosa.
- `ServiceLocatorBLL` (BLL/Helpers/ServiceLocatorBLL.cs) tiene la **única** responsabilidad de crear/proveer dependencias.
- En **GUI**, cada formulario cumple un rol: login, ABM usuarios, ABM roles, preferencias, cambio de contraseña e historial.

### O — Open/Closed (Abierto/Cerrado)

El sistema está **abierto a la extensión y cerrado a la modificación**:

- **Composite**: se pueden agregar nuevos tipos de componentes de rol/permiso sin modificar la interfaz `IRoleComponentBE` ni el recorrido del árbol.
- **Decorator**: se agrega la responsabilidad de "guardar en el historial" (`ActivityLoggingDecorator`) **sin tocar** la actividad base (`BaseActivity`); se pueden añadir nuevas actividades o decoradores sin modificar lo existente.
- **Strategy**: se puede incorporar un tercer algoritmo de hash (p. ej. Argon2) creando una nueva `IPasswordStrategy`, **sin modificar** ni `PasswordHasher` ni a sus consumidores.
- **Interfaces** `IUserBLL`, `IRoleBLL`, `IAuthBLL`, `IActivityBLL` y `IUserMPP`, `IRoleMPP`, `IActivityMPP`: el consumidor depende de la abstracción, por lo que se pueden incorporar nuevas implementaciones (p. ej. otra persistencia) sin alterar a quien las usa.

### L — Liskov Substitution (Sustitución de Liskov)

Los subtipos son intercambiables por su base sin romper el comportamiento:

- `RoleCompositeBE` y `PermissionLeafBE` pueden usarse indistintamente como `IRoleComponentBE`; el árbol los recorre de forma uniforme en `HasPermission` y `GetAllPermissions`.
- Las implementaciones concretas (p. ej. `UserMPP`, `RoleBLL`) son **sustituibles por mocks** en las pruebas (MSTest + Moq) sin que cambie el comportamiento de quienes las consumen.

### I — Interface Segregation (Segregación de Interfaces)

Interfaces **chicas y específicas por responsabilidad**: ningún cliente depende de métodos que no usa.

- `IAuthBLL` expone solo `Login`/`Logout`; `IActivityBLL` solo el historial; `IUserBLL` solo usuarios; `IRoleBLL` solo roles.
- En **MPP**, una interfaz por entidad (`IUserMPP`, `IRoleMPP`, `IActivityMPP`) en lugar de una interfaz de persistencia "gorila" que agrupe todo.

### D — Dependency Inversion (Inversión de Dependencias)

Las capas superiores dependen de **abstracciones**, no de concreciones:

- **BLL** consume `IUserMPP`, `IRoleMPP` e `IActivityMPP`; **GUI** consume las interfaces de BLL.
- **Inyección por constructor**: `AuthBLL(IUserMPP)`, `UserBLL(IUserMPP)`, `RoleBLL(IRoleMPP)`, `ActivityBLL(IActivityMPP)` (p. ej. BLL/Services/AuthBLL.cs). Esto habilita las pruebas con mocks y desacopla la creación, que queda centralizada en `ServiceLocatorBLL`.
- **Strategy**: `PasswordHasher` depende de `IPasswordStrategy` (puede recibir estrategias por constructor), lo que permite sustituir el algoritmo en pruebas sin tocar el contexto.
- La **dirección de dependencia** va de las capas altas hacia lo abstracto; el dominio (BE) no depende de nada.

### Limitaciones y puntos de mejora

Para no sobrevender el cumplimiento, cabe notar algunas decisiones que presentan margen de mejora respecto de los principios:

- `ServiceLocatorBLL` es un **Service Locator estático**: aunque los constructores reciben abstracciones (buen uso de DIP), el *cableado* de dependencias queda oculto y es difícil de sustituir en pruebas sin el patrón.
- `PermissionBLL` se expone **concreto** (sin interfaz) a través de `ServiceLocatorBLL.CreatePermissionBLL()`, a diferencia del resto de los servicios que se devuelven por su interfaz — una inconsistencia menor con DIP.
- Las capas **MPP** dependen directamente de la clase concreta `AccessDAL` (DAL); para un desacople total convendría que DAL también expusiera una abstracción.

## Cómo ejecutar

1. Tener instalado el **.NET SDK 10** (o **Visual Studio 2022 17.12+**, que lo incluye, para poder editar el diseñador WinForms).
2. Configurar la cadena de conexión `cadenaConexion` (proyecto **GUI**, archivo `App.config`) apuntando a una instancia de SQL Server con la base `Trabajo_Final`.
3. Ejecutar los scripts de [`sql/`](../../sql/) en orden (véase el README global). El esquema incluye la tabla `ActivityLogs` del **Historial de Actividad**; si la base ya existía, ejecutar el `CREATE TABLE [dbo].[ActivityLogs]` correspondiente (ver `01_CreateTables.sql`).
4. Compilar y ejecutar:

```
dotnet build TrabajoFinal-DarioZubaray.slnx -c Debug
dotnet run --project GUI\GUI.csproj
```

Usuarios de prueba: `admin` / `123` (rol Admin), `pepe` / `123` (rol Alumno).

El **acceso a datos** usa `Microsoft.Data.SqlClient` (SQL Server) y `System.Configuration.ConfigurationManager` para leer `cadenaConexion` desde el `App.config`.

## Pruebas

Los proyectos **BLL.Tests** y **MPP.Tests** (MSTest + Moq) cubren la autenticación, el cifrado de contraseñas, la gestión de idioma, la lógica de usuarios, el historial de actividad (Decorator y paginación) y la persistencia. Para ejecutarlos:

```
dotnet test TrabajoFinal-DarioZubaray.slnx
```

> **Nota:** `BLL.Tests` corre sin base de datos (usa mocks). `MPP.Tests` son pruebas de integración y requieren una instancia local de SQL Server (`PAPI-RYZEN3\SQLEXPRESS`).
