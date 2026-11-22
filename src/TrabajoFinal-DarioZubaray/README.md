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
| **BE** (*Business Entities*) | Entidades del dominio: `UserBE`, `RoleBE`, `PermissionBE`, `ClassroomBE`, `CourseBE`, `EnrollmentBE`, `AttendanceBE`, `ActivityLogEntryBE`. Contiene además el **DTO** `LoginResultBE` y la implementación del patrón **Composite** de roles. No depende de ninguna otra capa. |
| **DAL** (*Data Access Layer*) | Acceso a datos de bajo nivel. `AccessDAL` encapsula la conexión y ejecución de sentencias SQL (lectura, escalar y guardado) contra **SQL Server**, usando `SqlConnection`/`SqlCommand` y parámetros. |
| **MPP** (*Mapper*) | Capa de **mapeo** entre la base de datos y el modelo de negocio. `UserMPP`, `RoleMPP`, `ActivityMPP`, `ClassroomMPP`, `CourseMPP`, `EnrollmentMPP` y `AttendanceMPP` implementan interfaces, lo que permite inyectar mocks en pruebas. |
| **BLL** (*Business Logic Layer*) | Lógica y reglas de negocio. Se organiza en **Servicios** (`AuthBLL`, `UserBLL`, `RoleBLL`, `PermissionBLL`, `ActivityBLL`, `ClassroomBLL`, `CourseBLL`, `EnrollmentBLL`, `AttendanceBLL`), **Helpers** (`EncryptionBLL`, `CultureHelperBLL`, `SessionManagerBLL`, `AppPreferencesBLL`) y **ServiceLocatorBLL** (localizador/singleton de servicios). |
| **GUI** | Interfaz gráfica en WinForms: formularios de login, principal, usuarios, roles, aulas, cursos, inscripciones, asistencia, preferencias y cambio de contraseña. El punto de entrada es `Program.cs`. |
| **BLL.Tests** / **MPP.Tests** | Proyectos de prueba unitaria (MSTest + Moq). Cubren la lógica de `AuthBLL`, `UserBLL`, `RoleBLL`, `PermissionBLL`, `ActivityBLL`, `EncryptionBLL`, `CultureHelperBLL`, `SessionManagerBLL`, `AppPreferencesBLL`, `ClassroomBLL`, `CourseBLL`, `EnrollmentBLL`, `AttendanceBLL` y la persistencia de `UserMPP`, `RoleMPP`, `ActivityMPP`, `ClassroomMPP`, `CourseMPP`, `EnrollmentMPP`, `AttendanceMPP`. |

#### Dependencias entre proyectos

- **GUI** → referencia a `BE` y `BLL`.
- **BLL** → referencia a `BE` y `MPP`.
- **MPP** → referencia a `BE` y `DAL`.
- **BE** → sin dependencias.
- **BLL.Tests** → `BLL`, `BE`, `MPP` (+ `Moq`). **MPP.Tests** → `MPP`, `BE`, `DAL`, `BLL`.

## Descripción de clases por capa

### BE — Business Entities

| Clase | Responsabilidad |
|-------|-----------------|
| `UserBE` | Entidad *Usuario*. Modela datos de cuenta: nombre de usuario, hash de contraseña, estado activo, reintentos, fechas, idioma y rol asignado. Expone `GetDvhValues()` con la **composición de la fila** (qué valores y en qué orden entran) para el **Dígito Verificador Horizontal** (`Dvh`); la serialización canónica y el hasheo viven en `BLL.DVHHelperBLL`. |
| `RoleBE` | Entidad *Rol*. Posee la lista de permisos y el método `ToComposite(...)` que convierte el rol (y sus roles hijos) en un árbol **Composite**. |
| `PermissionBE` | Entidad *Permiso*. Identifica un formulario/acción concreta (`Name`, `Label`, `Description`, `IsSystem`). `IsSystem` marca permisos que no pueden quitarse. |
| `LoginResultBE` (DTO) | Resultado del inicio de sesión: `Success`, `Message` y `User` autenticado. |
| `ActivityLogBE` | Entidad que representa un registro del **Historial de Actividad**: usuario, acción, formulario, detalle y fecha/hora. |
| `ClassroomBE` | Entidad *Aula*. Modela un espacio físico: nombre y capacidad. |
| `CourseBE` | Entidad *Curso*. Modela un curso de idiomas: nombre, descripción, fechas de inicio/fin, aula asignada, día de la semana, horarios y lista de docentes. |
| `EnrollmentBE` | Entidad *Inscripción*. Modela la inscripción de un alumno a un curso con datos de horario y aula. |
| `AttendanceBE` | Entidad *Asistencia*. Modela un registro de asistencia: curso, alumno, fecha y estado (presente/ausente). |
| `IActivityLogEntryBE` | Interfaz componente del patrón **Decorator**: define `UserId`, `Action`, `FormName`, `Description` y `Execute()`. |
| `ActivityLogEntryBE` | Componente concreto del **Decorator**: representa una actividad concreta (acceso a formulario, login o logout) sin efectos secundarios. |
| `RoleComponentBE` | Entidad **genérica** del **Composite** (clase base abstracta). Define `Name`, `HasPermission(name)` y `GetAllPermissions()`. Es la base de la que heredan las 2 clases del patrón. |
| `RoleCompositeBE` | Nodo **compuesto** del árbol (hereda de `RoleComponentBE`): representa un rol que contiene hijos (otros roles y hojas de permiso). Permite agregar/quitar hijos y recorre el árbol para resolver permisos. |
| `PermissionLeafBE` | **Hoja** del árbol (hereda de `RoleComponentBE`): envuelve un `PermissionBE` concreto. Evalúa `HasPermission` contra su propio nombre. |
| `IntegrityViolationBE` (DTO) | Fila violada detectada por la auditoría: tabla, id, descripción, hash almacenado y hash recalculado. |

### DAL — Data Access Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `AccessDAL` | Abstracción del acceso a SQL Server. Lee la cadena de conexión `cadenaConexion` de la configuración y ofrece `Read`, `ReadScalar` y `Save` (con parámetros opcionales). |

### MPP — Modelo de Procedimientos de Persistencia (capa de mapeo)

| Clase | Responsabilidad |
|-------|-----------------|
| `IUserMPP` / `UserMPP` | Mapeo de usuarios: transforma filas (`DataRow`) en `UserBE` y viceversa, y persiste las operaciones de login (último acceso, reintentos, desactivación), CRUD, idioma y contraseña. **No calcula el `dvh`**: escribe, relee la fila persistida (para que BLL calcule el hash sobre los valores guardados, evitando el redondeo de `DATETIME`) y expone `UpdateDvh(id, dvh)` para persistir el hash que calcula BLL. |
| `IRoleMPP` / `RoleMPP` | Mapeo de roles y permisos: transforma filas en `RoleBE`/`PermissionBE`, CRUD de roles, permisos por rol, jerarquía padre-hijo (`RoleHierarchy`) y asignación de permisos. |
| `IActivityMPP` / `ActivityMPP` | Mapeo del historial de actividad: transforma filas en `ActivityLogBE`, inserta registros y consulta **paginada** (con `OFFSET`/`FETCH`) filtrando por usuario. |
| `IClassroomMPP` / `ClassroomMPP` | Mapeo de aulas: CRUD de aulas físicas con búsqueda por nombre y conteo. |
| `ICourseMPP` / `CourseMPP` | Mapeo de cursos: CRUD, asignación de docentes (N:N), validación de traslape de aula y de docente, búsqueda por docente y alumno, incluye cursos inactivos. |
| `IEnrollmentMPP` / `EnrollmentMPP` | Mapeo de inscripciones: inscripción/desinscripción de alumnos a cursos, búsqueda por alumno, conteo y validación de traslape de horario. |
| `IAttendanceMPP` / `AttendanceMPP` | Mapeo de asistencia: registro masivo (bulk), búsqueda por curso y fecha, y verificación de si un usuario es docente del curso. |

### BLL — Business Logic Layer

| Clase | Responsabilidad |
|-------|-----------------|
| `IAuthBLL` / `AuthBLL` | Autenticación. Valida credenciales, maneja hasta **3 reintentos**, desactiva el usuario superado el máximo y registra el último acceso; tras cada mutación **regenera el `dvh`** de la fila (`DVHHelperBLL.Regenerate`). El usuario **`admin`** está **exento** del bloqueo (no cuenta reintentos ni se desactiva) y requiere **doble factor (2FA)** antes de crear la sesión (`ValidateTwoFactor`). |
| `ITwoFactorBLL` / `InternalTwoFactorBLL` | Servicio de **doble factor de autenticación**. Contrato intercambiable para conectar proveedores reales (Google, Microsoft, etc.); hoy resuelve internamente con el código hardcodeado **`123`**. Se configura en `ServiceLocatorBLL.CreateTwoFactorBLL()`. |
| `IUserBLL` / `UserBLL` | Lógica de gestión de usuarios: CRUD, búsqueda, cambio de idioma y cambio de contraseña (verificando la actual). Tras **cada escritura regenera el `dvh`** (`DVHHelperBLL.Regenerate`: recalcula el hash sobre la fila releída y lo persiste vía `IUserMPP.UpdateDvh`). **Protege al usuario `admin`** (`UserBLL.ADMIN_USERNAME`, identificación por nombre case-insensitive): no se puede eliminar, desactivar, renombrar ni cambiarle el rol (`Delete`/`Save`); el rol es lo que define los permisos y por ende el menú de administración. En la UI, nombre/rol/estado quedan bloqueados al editar al admin (`UserForm`). |
| `IRoleBLL` / `RoleBLL` | Lógica de gestión de roles: CRUD, permisos por rol (protegiendo los de sistema) y jerarquía de roles. |
| `PermissionBLL` | Construye el árbol **Composite** de un rol mediante `BuildRoleTree(roleId)` y consulta permisos sobre el árbol (`HasPermission`). |
| `IActivityBLL` / `ActivityBLL` | Servicio del **Historial de Actividad**. Registra accesos a formularios y el inicio/cierre de sesión (usando el **Decorator**) y consulta el historial de forma paginada. |
| `IClassroomBLL` / `ClassroomBLL` | Lógica de gestión de aulas: CRUD, búsqueda y conteo. |
| `ICourseBLL` / `CourseBLL` | Lógica de gestión de cursos: CRUD, validación de traslape de aula al guardar, validación de traslape de docente, gestión de docentes asignados, búsqueda por docente y alumno. |
| `IEnrollmentBLL` / `EnrollmentBLL` | Lógica de inscripciones: inscripción con validación de duplicado y traslape de horario, desinscripción, búsqueda y conteo. |
| `IAttendanceBLL` / `AttendanceBLL` | Lógica de asistencia: registro masivo, búsqueda por curso/fecha y verificación de docente. |
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
| `AppPreferencesBLL` (static) | Persistencia local de preferencias de la UI (`language`, `theme`) en `%LocalAppData%\TrabajoFinal-DarioZubaray\preferences.json`. Conserva lo "último usado" entre sesiones. |
| `IIntegrityBLL` / `IntegrityBLL` | Servicio de **auditoría de integridad (DVH)**: `Scan()` recorre todos los verificadores registrados y guarda el resultado en `IntegrityStateBLL`; `GetViolations()` lista las filas alteradas; `AcceptAndRegenerate(violation)` reacepta una fila (vuelve a escanear). |
| `IDvhTableVerifierBLL` / `UserDvhVerifierBLL` / `EvaluationDvhVerifierBLL` / `GradeDvhVerifierBLL` | Verificadores por tabla (`users`, `evaluations` y `grades`): recalculan el DVH de cada fila (`DVHHelperBLL.Compute`) y comparan contra el persistido; `AcceptAndRegenerate` regenera el hash de una fila aceptada. Diseño multi-tabla: `ServiceLocatorBLL.CreateIntegrityBLL()` registra un verificador por cada tabla soportada. |
| `DVHHelperBLL` (static) | Helper del **Dígito Verificador Horizontal**: serializa los valores que declara la entidad (coma como separador, fechas estilo 126, `NULL` → `''`), calcula el **SHA-256** en hex (UTF-16LE, igual que `HASHBYTES` sobre `NVARCHAR`) y orquesta la regeneración (`Regenerate(mpp, user/evaluation/grade)`, `RegenerateEvaluation(mpp, id)`, `RegenerateGrade(mpp, id)`) tras cada escritura. |
| `IntegrityStateBLL` (static) | Memoria de la sesión con el resultado del último escaneo (`LastScanHasFailures`, `RecordScan`, `Reset`), consumida por `LoginForm` y `MainForm`. |

### GUI — Interfaz gráfica

| Formulario | Responsabilidad |
|------------|-----------------|
| `LoginForm` | Autenticación. Crea la sesión (`SessionManagerBLL.CreateSession`), abre el menú principal y la limpia al cerrar (`RemoveSession`). Al iniciar y al cerrar sesión aplica el idioma y tema "último usado" (`AppPreferencesBLL`). Si el login requiere 2FA (`RequiresTwoFactor`, usuario `admin`), pide el código en `TwoFactorForm` antes de crear la sesión. Al conectar con la base y en **cada intento de login** ejecuta el **escaneo de integridad (DVH)** en segundo plano; si al lanzar hay filas alteradas muestra un aviso modal. Con filas alteradas, los usuarios no-sysadmin quedan bloqueados (`INT-001`) y el sysadmin (tras el 2FA) pasa por la auditoría antes de entrar. |
| `RegisterForm` | **Alta de la cuenta administradora (CU-03)**, mostrada por el arranque cuando la base no tiene usuarios. Valida usuario/contraseña/reingreso (`VAL-001`, `VAL-002`, `VAL-003`, `VAL-008`), asigna el rol Admin, guarda con hash y abre `MainForm` con la sesión creada; al cerrar la sesión vuelve a `LoginForm`. |
| `MainForm` | Menú principal (MDI). Muestra el usuario/rol en el pie y **oculta o muestra opciones según los permisos** del árbol Composite de la sesión. |
| `UserManagementForm` / `UserForm` | ABM de usuarios: listado, búsqueda, alta, edición y baja lógica. |
| `RoleManagementForm` | ABM de roles y asignación de permisos (manteniendo los de sistema). |
| `ClassroomManagementForm` / `ClassroomForm` | ABM de aulas: listado, búsqueda, alta, edición y baja lógica. |
| `CourseManagementForm` / `CourseForm` | ABM de cursos: listado, búsqueda, alta, edición y baja lógica. Incluye selección de docentes (N:N) y validación de disponibilidad de aula. Si no hay aulas creadas, muestra un mensaje inline. |
| `EnrollmentManagementForm` | Inscripciones: el Coordinador inscribe alumnos a cursos activos y puede desinscribir. Valida duplicado y traslape de horario. |
| `AttendanceForm` / `AttendanceViewForm` | Registro de asistencia (docente) / visualización de asistencia (alumno). |
| `PreferencesForm` | Cambio de idioma y tema; actualiza la sesión y refresca los recursos de la UI. También persiste lo seleccionado en `AppPreferencesBLL`. |
| `ChangePasswordForm` | Cambio de contraseña validando la actual. |
| `ActivityHistoryForm` | **Historial de Actividad** (dentro del menú Archivo): lista paginada de las actividades del usuario autenticado. |
| `IntegrityAuditForm` | **Auditoría de Integridad (DVH)**: grilla de filas alteradas (tabla, id, hash almacenado vs recalculado) con acciones *Editar*, *Aceptar y regenerar*, *Refrescar* y *Cerrar*. Accesible desde el login (si el escaneo falló) y desde el menú principal (solo sysadmin). |
| `StudentManagementForm` / `StudentForm` | ABM de alumnos (entidad `students`; no son usuarios del sistema). |
| `TicketManagementForm` / `TicketForm` | Tickets de soporte: alta (Coordinador y Profesor) y listado con cierre (Coordinador). |
| `EvaluationForm` | Evaluaciones y notas de los cursos del docente. |
| `PerformanceForm` / `ReportForm` | Reportes: rendimiento académico por curso e informes personalizados con exportación CSV. |
| `BackupForm` | Respaldo y restauración de la base de datos. |
| `TwoFactorForm` | Segundo factor de autenticación (código 2FA) para el usuario `admin`. |
| `AboutForm` | Diálogo Acerca de. |

## Patrones de diseño destacados

### 1) Composite — jerarquía de roles y permisos

El patrón **Composite** permite tratar por igual a un rol completo y a un permiso individual, de modo que los permisos de un usuario se modelan como un **árbol**.

```
RoleComponentBE  (entidad genérica abstracta)
   ├─ RoleCompositeBE  (compuesto: contiene hijos)
   └─ PermissionLeafBE (hoja: envuelve un PermissionBE)
```

Participantes:

- **Componente (entidad genérica)** → `RoleComponentBE` — clase base abstracta con `Name`, `HasPermission(name)`, `GetAllPermissions()`.
- **Compuesto** → `RoleCompositeBE` (hereda de `RoleComponentBE`) — mantiene una lista `_children` y dispone de `AddChild`/`RemoveChild`. `HasPermission` delega en cada hijo y `GetAllPermissions` concatena los resultados.
- **Hoja** → `PermissionLeafBE` (hereda de `RoleComponentBE`) — envuelve un `PermissionBE` concreto; `HasPermission` compara su propio nombre.

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
    private static IUserMPP _userMPP;       // única instancia
    private static IRoleMPP _roleMPP;       // única instancia
    private static IActivityMPP _activityMPP;
    private static IClassroomMPP _classroomMPP;
    private static ICourseMPP _courseMPP;
    private static IEnrollmentMPP _enrollmentMPP;
    private static IAttendanceMPP _attendanceMPP;

    public static IUserMPP GetUserMPP()
    {
        if (_userMPP == null) _userMPP = new UserMPP();  // lazy singleton
        return _userMPP;
    }
    // ... GetRoleMPP(), GetActivityMPP(), GetClassroomMPP(), GetCourseMPP(), etc.
    // ... CreateAuthBLL(), CreateUserBLL(), CreateRoleBLL(), CreateClassroomBLL(), etc.
}
```

- Garantiza **una sola instancia** de cada servicio de persistencia durante todo el ciclo de vida de la aplicación (*singleton*).
- **Descarga la responsabilidad** de instanciación de las formas de la GUI: `LoginForm`, `UserForm`, `UserManagementForm`, `RoleManagementForm`, `ChangePasswordForm`, `PreferencesForm`, `ClassroomForm`, `ClassroomManagementForm`, `CourseForm`, `CourseManagementForm`, `EnrollmentManagementForm`, `AttendanceForm` y `AttendanceViewForm` obtienen sus servicios a través de `ServiceLocatorBLL`.

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
IActivityLogEntryBE  (interfaz componente: Execute())
   ├─ ActivityLogEntryBE            (componente concreto: actividad de acceso formulario / login / logout)
   └─ ActivityLoggingDecoratorBLL   (decorator: ejecuta la actividad y luego guarda en la base)
```

Participantes:

- **Componente** → `IActivityLogEntryBE` — `UserId`, `Action`, `FormName`, `Description` y `Execute()`.
- **Componente concreto** → `ActivityLogEntryBE` — una actividad concreta; `Execute()` no produce efectos secundarios.
- **Decorator** → `ActivityLoggingDecoratorBLL` — recibe una `IActivityLogEntryBE` y un `IActivityMPP`; en `Execute()` primero ejecuta la actividad envuelta y luego **guarda el registro** (`_activityMPP.Save(...)`).

**Uso**: `ActivityBLL` expone `LogFormAccess`, `LogLogin` y `LogLogout`, que construyen `new ActivityLoggingDecoratorBLL(new ActivityLogEntryBE(...), _activityMPP).Execute()`. Los puntos de registro son:

- **LoginForm**: tras autenticar (`LogLogin`) y al cerrar sesión (`LogLogout`).
- **MainForm**: cada opción del menú que abre un formulario registra un `LogFormAccess` (Preferencias, Usuarios, Roles, Cambiar Contraseña).
- **ActivityHistoryForm**: lista paginada de las actividades del usuario autenticado (no se registra a sí misma, para evitar ruido).

### 5) Strategy — algoritmos de hash de contraseñas

El **Strategy** permite intercambiar algoritmos en tiempo de ejecución sin modificar a quien los consume. Se usa para la verificación de contraseñas, que históricamente mezclaba dos algoritmos en un único `if/else` y hoy delega en estrategias intercambiables.

```
IPasswordStrategyBLL  (interfaz estrategia: Matches / Hash / Verify)
   ├─ BcryptPasswordStrategyBLL        (BCrypt)
   └─ LegacySha256PasswordStrategyBLL  (SHA-256 legacy, para datos existentes)
PasswordHasherBLL  (contexto: elige la estrategia según el formato del hash)
```

Participantes:

- **Estrategia** → `IPasswordStrategyBLL` — `Matches(storedHash)` (dice si el hash le corresponde), `Hash(password)` y `Verify(plain, stored)`.
- **Estrategia concreta** → `BcryptPasswordStrategyBLL` (BCrypt) y `LegacySha256PasswordStrategyBLL` (SHA-256 de 64 hex), esta última para mantener la compatibilidad con usuarios creados antes del cifrado BCrypt.
- **Contexto** → `PasswordHasherBLL` — mantiene una lista ordenada de estrategias; `Verify` recorre las estrategias, delega en la que `Matches` el hash guardado y, si ninguna, usa la estrategia por defecto (BCrypt).

**Uso**: `EncryptionBLL` (podado a ser una fachada de compatibilidad) delega en `PasswordHasherBLL.Default` para `HashPassword` y `VerifyPassword`; `AuthBLL`, `UserBLL` y `GUI/UserForm` siguen usando la misma API sin cambios. Agregar un tercer algoritmo (p. ej. Argon2) solo implica una nueva `IPasswordStrategyBLL`: **Open/Closed y sin tocar el contexto**.

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

- **Composite**: se pueden agregar nuevos tipos de componentes de rol/permiso sin modificar la clase base `RoleComponentBE` ni el recorrido del árbol.
- **Decorator**: se agrega la responsabilidad de "guardar en el historial" (`ActivityLoggingDecorator`) **sin tocar** la actividad base (`BaseActivity`); se pueden añadir nuevas actividades o decoradores sin modificar lo existente.
- **Strategy**: se puede incorporar un tercer algoritmo de hash (p. ej. Argon2) creando una nueva `IPasswordStrategyBLL`, **sin modificar** ni `PasswordHasherBLL` ni a sus consumidores.
- **Interfaces** `IUserBLL`, `IRoleBLL`, `IAuthBLL`, `IActivityBLL` y `IUserMPP`, `IRoleMPP`, `IActivityMPP`: el consumidor depende de la abstracción, por lo que se pueden incorporar nuevas implementaciones (p. ej. otra persistencia) sin alterar a quien las usa.

### L — Liskov Substitution (Sustitución de Liskov)

Los subtipos son intercambiables por su base sin romper el comportamiento:

- `RoleCompositeBE` y `PermissionLeafBE` pueden usarse indistintamente como `RoleComponentBE`; el árbol los recorre de forma uniforme en `HasPermission` y `GetAllPermissions`.
- Las implementaciones concretas (p. ej. `UserMPP`, `RoleBLL`) son **sustituibles por mocks** en las pruebas (MSTest + Moq) sin que cambie el comportamiento de quienes las consumen.

### I — Interface Segregation (Segregación de Interfaces)

Interfaces **chicas y específicas por responsabilidad**: ningún cliente depende de métodos que no usa.

- `IAuthBLL` expone solo `Login`/`Logout`; `IActivityBLL` solo el historial; `IUserBLL` solo usuarios; `IRoleBLL` solo roles.
- En **MPP**, una interfaz por entidad (`IUserMPP`, `IRoleMPP`, `IActivityMPP`) en lugar de una interfaz de persistencia "gorila" que agrupe todo.

### D — Dependency Inversion (Inversión de Dependencias)

Las capas superiores dependen de **abstracciones**, no de concreciones:

- **BLL** consume `IUserMPP`, `IRoleMPP` e `IActivityMPP`; **GUI** consume las interfaces de BLL.
- **Inyección por constructor**: `AuthBLL(IUserMPP, ITwoFactorBLL)`, `UserBLL(IUserMPP)`, `RoleBLL(IRoleMPP)`, `ActivityBLL(IActivityMPP)` (p. ej. BLL/Services/AuthBLL.cs). Esto habilita las pruebas con mocks y desacopla la creación, que queda centralizada en `ServiceLocatorBLL`.
- **Strategy**: `PasswordHasherBLL` depende de `IPasswordStrategyBLL` (puede recibir estrategias por constructor), lo que permite sustituir el algoritmo en pruebas sin tocar el contexto.
- La **dirección de dependencia** va de las capas altas hacia lo abstracto; el dominio (BE) no depende de nada.

### Limitaciones y puntos de mejora

Para no sobrevender el cumplimiento, cabe notar algunas decisiones que presentan margen de mejora respecto de los principios:

- `ServiceLocatorBLL` es un **Service Locator estático**: aunque los constructores reciben abstracciones (buen uso de DIP), el *cableado* de dependencias queda oculto y es difícil de sustituir en pruebas sin el patrón.
- `PermissionBLL` se expone **concreto** (sin interfaz) a través de `ServiceLocatorBLL.CreatePermissionBLL()`, a diferencia del resto de los servicios que se devuelven por su interfaz — una inconsistencia menor con DIP.
- Las capas **MPP** dependen directamente de la clase concreta `AccessDAL` (DAL); para un desacople total convendría que DAL también expusiera una abstracción.
- `EnrollmentBLL` recibe `ICourseBLL` por constructor (buen uso de DIP), pero `ServiceLocatorBLL` lo crea internamente, lo que dificulta el testing aislado sin el patrón.

## Manejo de errores y códigos de error

La aplicación centraliza el **catálogo de códigos de error** en la capa de negocio, de modo que todos los dominios comparten el mismo criterio de codificación, son fáciles de buscar y escalan ordenadamente.

### Catálogo centralizado (`BLL\Helpers\ErrorCodesBLL.cs`)

Los códigos se agrupan en **clases anidadas por dominio**, siguiendo el patrón `ErrorCodesBLL.<Dominio>.<Nombre>`:

| Prefijo | Dominio | Códigos |
|---------|---------|---------|
| `DB-` | `Database` | `Unavailable DB-001`, `Configuration DB-002`, `QueryFailed DB-003` |
| `AUTH-` | `Auth` | `RequiredFields AUTH-001`, `InvalidCredentials AUTH-002`, `UserBlocked AUTH-003`, `MaxRetriesExceeded AUTH-004`, `RetryAttemptsLeft AUTH-005`, `TwoFactorInvalid AUTH-006` |
| `VAL-` | `Validation` | `UsernameRequired VAL-001`, `PasswordRequired VAL-002`, `PasswordsMismatch VAL-003`, `NameRequired VAL-004`, `NoSelection VAL-005`, `LanguageThemeRequired VAL-006`, `AdminUsernameLocked VAL-007`, `UsernameTaken VAL-008` |
| `BIZ-` | `Business` | `RoleHasUsers BIZ-001`, `InvalidCurrentPassword BIZ-002`, `TraslapeHorario BIZ-003`, `YaInscripto BIZ-004`, `AdminProtected BIZ-005`, `TicketSubjectRequired BIZ-006`, `TicketDescriptionRequired BIZ-007`, `TicketAlreadyClosed BIZ-008`, `StudentFirstNameRequired BIZ-009`, `StudentLastNameRequired BIZ-010`, `StudentDocumentRequired BIZ-011`, `StudentDocumentDuplicated BIZ-012`, `EvaluationNameRequired BIZ-013`, `CourseNotOwnedByTeacher BIZ-014`, `GradeOutOfRange BIZ-015`, `EvaluationDateOutOfRange BIZ-016`, `AttendanceDateOutOfRange BIZ-017` |
| `INT-` | `Integrity` | `Blocked INT-001`, `ResolveFailed INT-002`, `ScanFailed INT-003` |
| `GEN-` | `General` | `Unhandled GEN-001` |

### Cómo se transportan y muestran

- La capa **BLL** asigna el código en un campo propio del resultado (`LoginResultBE.ErrorCode`), no encadenado al texto traducido. Así la presentación puede formatearlo sin depender del mensaje.
- La capa **GUI** agrega el código con el formato uniforme **(Código: XXX)** mediante el helper `ErrorFormatter.WithCode(...)` y la clave localizada `Error_CodeFormat`. Los mensajes visibles quedan p. ej. `Usuario o contraseña incorrectos. (Código: AUTH-002)`.
- Cada mensaje de usuario está **localizado** (`es` / `en` / `pt-BR`) en los `.resx` de **BE**, mientras el código es único e independiente del idioma (ideal para soporte técnico y registros).

### Puntos donde se aplica

| Formulario / Servicio | Código usado |
|-----------------------|--------------|
| `LoginForm` / `AuthBLL` | `DB-001`, `AUTH-001` a `AUTH-006` |
| `ChangePasswordForm` / `UserBLL.ChangePassword` | `VAL-002`, `VAL-003`, `BIZ-002` |
| `UserForm` | `VAL-001`, `VAL-002` |
| `UserManagementForm` | `VAL-005`, `BIZ-005` (+ título `Common_Confirm`) |
| `RoleManagementForm` | `VAL-004`, `VAL-005`, `BIZ-001` |
| `PreferencesForm` | `VAL-006` |
| `StudentForm` | `BIZ-009` a `BIZ-011` |
| `TicketForm` | `BIZ-006`, `BIZ-007` |
| `EvaluationForm` / `EvaluationBLL` | `BIZ-013` a `BIZ-015` |
| `LoginForm` / `IntegrityBLL` (auditoría DVH) | `INT-001` a `INT-003` |

### Excepciones inesperadas (handler global)

Para que ninguna excepción de las capas inferiores (por ejemplo `SqlException`, `NullReferenceException` de los mappers o `ConfigurationErrorsException` por cadena de conexión ausente) crashee la aplicación sin aviso, `Program.cs` registra **dos** capturas globales:

- `Application.ThreadException` — errores del hilo de UI (WinForms).
- `AppDomain.CurrentDomain.UnhandledException` — errores no manejados en otros hilos o antes del cierre.

Ambas muestran un mensaje amigable con el código genérico **(Código: GEN-001)** en lugar de cerrar la aplicación silenciosamente o mostrar un diálogo de runtime.

## Integridad de datos (Dígito Verificador Horizontal — DVH)

La aplicación protege las tablas `users`, `evaluations` y `grades` con un **Dígito Verificador Horizontal**: cada fila guarda en la columna `dvh` el **SHA-256** de todos sus valores (menos el propio hash).

### Algoritmo (`BLL\Helpers\DVHHelperBLL.cs`)

- **Composición de la fila:** la entidad declara qué valores y en qué orden entran al hash (`UserBE.GetDvhValues()` con `id`…`theme`; `EvaluationBE.GetDvhValues()` con `id`, `course_id`, `name`, `date`, `created_by`, `created_at`, `last_update`; `GradeBE.GetDvhValues()` con `id`, `evaluation_id`, `student_id`, `grade`; siempre excluido `dvh`); `role_id` `0`/`NULL` se declara como `NULL`.
- **Serialización canónica:** la helper normaliza cada valor y los une por **coma `,` sin espacios**: `NULL` → `''` (`role_id` `0`/`NULL` → `''`), `bool` → `1`/`0`, fechas en formato estilo 126 (`yyyy-MM-ddTHH:mm:ss.fff`, **sin fracción cuando los milisegundos son 0**, igual que `CONVERT(..., 126)` de T-SQL).
- **Hash:** SHA-256 en hex **minúsculas** (64 caracteres) del payload codificado en **UTF-16LE** (`Encoding.Unicode`), para coincidir byte a byte con `HASHBYTES('SHA2_256', <NVARCHAR>)` de SQL Server.

### Escritura regenerativa (BLL)

Toda escritura de la fila **regenera su `dvh`**: `UserBLL` y `AuthBLL` invocan `DVHHelperBLL.Regenerate(...)` tras cada mutación (`Save`, `Delete`, idioma, tema, contraseña, reintentos, último acceso, desactivación). La helper recalcula el hash sobre la fila **releída** desde la base (no sobre los valores en memoria, porque `DATETIME` redondea a 1/300 s al escribir) y lo persiste con `IUserMPP.UpdateDvh(id, dvh)`. `UserMPP` solo escribe y relee la fila: no calcula hashes.

`EvaluationBLL.Save` hace lo propio con evaluaciones y notas: tras insertar/actualizar, relee la evaluación (`RegenerateEvaluation`) y cada nota (`Regenerate`) y persiste los hashes vía `IEvaluationMPP.UpdateDvh` / `UpdateGradeDvh`. `EvaluationMPP` tampoco calcula hashes.

### Verificación al inicio y auditoría

1. Al conectar con la base y en cada intento de login, `LoginForm` ejecuta `IntegrityBLL.Scan()` en segundo plano: recalcula el DVH de cada fila registrada y lo compara con el persistido (`IntegrityStateBLL`). Si al lanzar la aplicación hay discrepancias, se muestra un aviso modal (mensaje `INT-001`); el re-escaneo del click de login es silencioso porque el bloqueo/auditoría ya informa en ese momento.
2. Si hay discrepancias, `AuthBLL.Login` **bloquea** a todo usuario que no sea el sysadmin (`admin`, id 1) con el código **`INT-001`**.
3. El sysadmin (tras su **2FA**) abre el formulario **Auditoría de Integridad** (`IntegrityAuditForm`), donde puede *Editar* la fila o *Aceptar y regenerar* su hash; el sistema vuelve a escanear y solo ingresa cuando no quedan discrepancias (o si el sysadmin confirma continuar con alertas). También puede reabrirla desde el menú principal.
4. Diseño **multi-tabla**: `ServiceLocatorBLL.CreateIntegrityBLL()` registra un `IDvhTableVerifierBLL` por tabla (`UserDvhVerifierBLL` para `users`, `EvaluationDvhVerifierBLL` para `evaluations` y `GradeDvhVerifierBLL` para `grades`); agregar una tabla nueva consiste en sumar un verificador.

La migración `sql/03_v1.3.0_Incremental.sql` agrega la columna `users.dvh` y calcula el backfill con la misma expresión T-SQL (verificada contra la app por el test `Dvh_BackfillExpression_MatchesApplicationHash`); `sql/06_v1.4.0_DVHGradesEvaluations.sql` hace lo propio con `evaluations.dvh` y `grades.dvh` (mismo contrato de serialización, con `CAST(date AS DATETIME)` antes del estilo `126` para igualar a la app), verificado por los tests `EvaluationDvhIntegrationTests` y `GradeDvhVerifierBLLTests`.

## Cómo ejecutar

1. Tener instalado el **.NET SDK 10** (o **Visual Studio 2022 17.12+**, que lo incluye, para poder editar el diseñador WinForms).
2. Configurar la cadena de conexión `cadenaConexion` (proyecto **GUI**, archivo `App.config`) apuntando a una instancia de SQL Server con la base `DarioZubaray_TF`.
3. Crear la base con los scripts de [`sql/`](../sql/) en orden (véase el README global), con el instalador, o con la CLI `--bootstrap` (más abajo). El esquema incluye la tabla `ActivityLogs` del **Historial de Actividad**; si la base ya existía, ejecutar el `CREATE TABLE [dbo].[ActivityLogs]` correspondiente (ver `01_v1.0.0_CreateTables.sql`).
4. Compilar y ejecutar:

```
dotnet build TrabajoFinal-DarioZubaray.sln -c Debug
dotnet run --project GUI\GUI.csproj
```

Al arrancar, `Program.cs` muestra un **splash** (`SplashForm`) mientras verifica la conexión en segundo plano y decide la primera pantalla: si la base **no tiene usuarios** muestra `RegisterForm` (CU-03) para crear la cuenta administradora; si ya hay usuarios, muestra `LoginForm` (CU-01). Si la verificación falla (servidor caído, instancia inexistente, base inexistente o esquema ausente), muestra un mensaje con el código **`DB-001`** y la aplicación se cierra (exit code `2`). La configuración de conexión no se abre al iniciar: puede consultarse desde `LoginForm` y `MainForm`.

Usuarios de prueba: `admin` / `admin` (rol Admin, código 2FA `123`), `dario` / `123` (Admin), `coord1` / `123` (Coordinador), `prof_brooks` / `123` (Profesor). La lista completa está en `src/sql/README.md`.

El **acceso a datos** usa `Microsoft.Data.SqlClient` (SQL Server) y `System.Configuration.ConfigurationManager` para leer `cadenaConexion` desde el `App.config`.

### CLI de mantenimiento

La aplicación acepta argumentos de línea de comandos (usados por el instalador):

```
TrabajoFinal-DarioZubaray.exe --test-connection   # 0 = OK, 2 = sin conexión
TrabajoFinal-DarioZubaray.exe --bootstrap [--sql-dir <ruta>]   # corre 01 + 03; 0 = OK, 1 = error, 2 = sin conexión, 3 = sin scripts
```

`--bootstrap` crea la base si no existe y ejecuta los scripts en orden (excluye `02`, `04` y `05`), de forma idempotente. La traza se guarda en `%TEMP%\TrabajoFinal-DarioZubaray_bootstrap.log`.

## Pruebas

Los proyectos **BLL.Tests** y **MPP.Tests** (MSTest + Moq) cubren la autenticación, el cifrado de contraseñas, la gestión de idioma, la lógica de usuarios, roles y permisos, el historial de actividad (Decorator y paginación), las preferencias locales, la estrategia de hash de contraseñas, la gestión de aulas, cursos, inscripciones y asistencia, y el **Dígito Verificador Horizontal** (serialización/normalización, bloqueo de login, auditoría y paridad con el backfill T-SQL). Para ejecutarlos:

```
dotnet test TrabajoFinal-DarioZubaray.sln
```

> **Nota:** `BLL.Tests` corre sin base de datos (usa mocks). `MPP.Tests` son pruebas de integración y requieren una instancia local de SQL Server (`PAPI-RYZEN3\SQLEXPRESS`).
