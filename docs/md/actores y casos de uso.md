
### Actores:
- sysadmin: el usuario `admin` (id 1) con `UserBLL.IsSysAdmin`; es un concepto del código, no un rol de la BD
- administrador: abm app
- coordinador: abm logica negocio
- profesor: operador logica negocio
- director: visor para reportes

### Casos de Usos:
- CU-01 Login 
- CU-02 Logout
- CU-03 Register

- CU-04 Gestionar Usuarios
- CU-05 Gestionar Roles y Permisos

- CU-06 Crear Aula
- CU-07 Crear Curso
- CU-08 Crear Alumno
- CU-09 Crear Inscripción
- CU-10 Crear Tickets

- CU-11 Registrar Asistencia
- CU-12 Registrar Evaluación y Notas
  
- CU-13 Consultar Rendimiento Académico
- CU-14 Consultar Informe Personalizado
- CU-15 Gestionar Respaldo y Restauración de la base de datos

### CU por Actores:

- Administrador
	CU-01 Login 
	CU-02 Logout
	CU-03 Register
	CU-04 Gestionar Usuarios
	CU-05 Gestionar Roles y Permisos

- Coordinador
	CU-01 Login
	CU-02 Logout
	CU-06 Crear Aula
	CU-07 Crear Curso
	CU-08 Crear Alumno
	CU-09 Crear Inscripción
	CU-10 Crear Tickets

- Profesor
	CU-01 Login
	CU-02 Logout
	CU-10 Crear Tickets
	CU-11 Registrar Asistencia
	CU-12 Registrar Evaluación y Notas

- Director
	CU-01 Login
	CU-02 Logout
	CU-13 Consultar Rendimiento Académico
	CU-14 Consultar Informe Personalizado

### Menu por CU:

Leyenda: "sin CU" = funcionalidad auxiliar sin caso de uso propio; "-" = sin permiso (la Auditoría se controla por sysadmin, no por permisos).

| Menu | MenuItem | Shortcut | CU | Permiso(s) | Rol(es) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| Archivo | Preferencias | - | sin CU | FORM_PREFERENCES | todos |
| Archivo | Cambiar contraseña | - | sin CU | FORM_CHANGE_PASS | todos |
| Archivo | Historial de actividad | - | sin CU | FORM_ACTIVITY_HISTORY | todos |
| Archivo | Cerrar sesión | - | CU-02 Logout | FORM_LOGOUT | todos |
| Administración | Usuarios | Ctrl+U | CU-04 Gestionar Usuarios | FORM_USER_MGMT | Admin |
| Administración | Roles y permisos | Ctrl+R | CU-05 Gestionar Roles y Permisos | FORM_ROLE_MGMT | Admin |
| Administración | Alumnos | Ctrl+A | CU-08 Crear Alumno | FORM_STUDENT_MGMT | Admin, Coordinador |
| Administración | Aulas | Ctrl+X | CU-06 Crear Aula | FORM_CURSO_MGMT (compartido con Cursos) | Admin, Coordinador |
| Administración | Cursos | Ctrl+C | CU-07 Crear Curso | FORM_CURSO_MGMT | Admin, Coordinador |
| Administración | Inscripciones | Ctrl+I | CU-09 Crear Inscripción | FORM_INSCRIPCION_MGMT | Admin, Coordinador |
| Administración | Auditoría de integridad | - | sin CU | - (gate duro `UserBLL.IsSysAdmin`, no es permiso) | solo sysadmin (admin id 1) |
| Administración | Respaldo y restauración | - | CU-15 Gestionar Respaldo y Restauración de la base de datos | FORM_BACKUP_MGMT | Admin |
| Soporte | Tickets | - | CU-10 Crear Tickets | FORM_TICKET_CREATE + FORM_TICKET_MGMT (un mismo formulario con caminos alternativos) | CREATE: Coordinador, Profesor / MGMT: Admin, Coordinador |
| Académico | Asistencia | - | CU-11 Registrar Asistencia | FORM_ASISTENCIA_MGMT + FORM_ASISTENCIA_VIEW (dos formularios, un ítem de menú) | MGMT: Admin, Profesor / VIEW: Admin, Coordinador |
| Académico | Evaluaciones | - | CU-12 Registrar Evaluación y Notas | FORM_EVAL_MGMT | Admin, Profesor |
| Reportes | Rendimiento académico | - | CU-13 Consultar Rendimiento Académico | FORM_REPORTS | Admin, Director |
| Reportes | Informes personalizados | - | CU-14 Consultar Informe Personalizado | FORM_REPORT_DETAIL | Admin, Director |
| Ayuda | Acerca de | - | sin CU | FORM_ABOUT | todos |

Notas:
- Un ítem de menú puede tener varios permisos (formularios con caminos alternativos): Tickets y Asistencia son los dos casos.
- Los 5 permisos base (Preferencias, Cambiar contraseña, Historial de actividad, Cerrar sesión, Acerca de) son `is_system=0` y se otorgan a todos los roles; se verifican con `EnsurePermission` igual que el resto.
- CU-01 Login y CU-03 Register no tienen ítem de menú (se resuelven en el `LoginForm`); Preferencias, Cambiar contraseña, Historial de actividad y Acerca de sí tienen ítem de menú pero no CU.
- Orden de Administración y shortcuts coinciden con `MainForm.Designer.cs`.

### Usuarios por Role:
- Administrador
- Coordinador
- Profesor
- Director