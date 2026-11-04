
Actores:
- sudo (existe en la BD y código pero no se documenta)
- administrador: abm app
- coordinador: abm logica negocio
- profesor: operador logica negocio
- director: visor para reportes

Casos de Usos:
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

CU por Actores:

Administrador
	CU-01 Login 
	CU-02 Logout
	CU-03 Register
	CU-04 Gestionar Usuarios
	CU-05 Gestionar Roles y Permisos
	CU-15 Gestionar Respaldo y Restauración de la base de datos

Coordinador
	CU-01 Login
	CU-02 Logout
	CU-06 Crear Aula
	CU-07 Crear Curso
	CU-08 Crear Alumno
	CU-09 Crear Inscripción
	CU-10 Crear Tickets

Profesor
	CU-01 Login
	CU-02 Logout
	CU-10 Crear Tickets
	CU-11 Registrar Asistencia
	CU-12 Registrar Evaluación y Cargar Notas

Director
	CU-01 Login
	CU-02 Logout
	CU-13 Consultar Rendimiento Académico
	CU-14 Consultar Informe Personalizado