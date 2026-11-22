=============================
Usernames
=============================
Administrador (rol Admin) - alto: tareas de inicio / excepcionales
	admin|admin			<- usuario sysadmin de la base: exento de bloqueo,
					   requiere 2FA (codigo 123) y es el unico
					   que ve la Auditoria de Integridad (DVH)
	dario|123

Coordinador (rol Coordinador) - medio: tareas diarias de la academia
	coord1|123
	coord2|123
	coord3|123

Profesor (rol Profesor) - bajo: operador de input (asistencia / notas)
	prof_brooks|123
	prof_omh|123
	prof_conway|123
	prof_pareto|123
	prof_amdahl|123
	prof_kirchhoff|123
	prof_murphy|123
	prof_newton|123

Director (rol Director) - solo lectura de reportes
	sin usuarios seed de prueba; crear desde CU-04 si hace falta

=============================
Casos de uso por rol
=============================
Administrador
	CU-01 Login | CU-02 Logout
	CU-03 Register (primera ejecución)
	CU-04 Crear Usuario | CU-05 Editar Usuario | CU-06 Eliminar Usuario (Buscar | Desbloquear)
	CU-07 Crear Rol | CU-08 Editar Rol y Permisos | CU-09 Eliminar Rol
	CU-10 Crear Aula | CU-11 Crear Curso | CU-12 Crear Alumno (entidad students, no usuarios) | CU-13 Crear Inscripcion
	CU-14 Crear Ticket | CU-15 Cerrar Ticket (gestion: listar todos y cerrar)
	CU-20 Crear Respaldo | CU-21 Restaurar Respaldo
	Auditoria de Integridad (DVH)

Coordinador
	CU-01 Login | CU-02 Logout
	CU-10 Crear Aula
	CU-11 Crear Curso
	CU-12 Crear Alumno (entidad students, no usuarios)
	CU-13 Crear Inscripcion
	CU-14 Crear Ticket | CU-15 Cerrar Ticket (gestion: listar todos y cerrar)
	Consulta de asistencia (solo lectura)

Profesor
	CU-01 Login | CU-02 Logout
	CU-14 Crear Ticket (crear)
	CU-16 Registrar Asistencia (solo sus cursos)
	CU-17 Registrar Evaluacion y Notas (solo sus cursos)

Director
	CU-01 Login | CU-02 Logout
	CU-18 Consultar Rendimiento Academico
	CU-19 Consultar Informe Personalizado

=============================
Alumno
=============================
No es un rol del sistema ni un actor. Es una entidad gestionada en la
tabla [students] (ABM con permiso FORM_STUDENT_MGMT: Coordinador y
Admin). Sus inscripciones y su asistencia los registra la academia.
Por eso no tiene casos de uso ni permisos de acceso.
