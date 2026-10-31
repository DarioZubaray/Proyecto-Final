# Documentación (`docs/`)

Esta carpeta reúne material de documentación del trabajo final: **diagramas** en formato Mermaid, modelos UML de **Enterprise Architect** y documentación de la cursada.

## Contenido

| Subcarpeta | Descripción |
|------------|-------------|
| [`mermaid-live/`](mermaid-live/) | Diagramas en formato [Mermaid](https://mermaid.js.org/): casos de uso, clases, entidad-relación y secuencias de login/logout. |
| [`md/`](md/) | Especificaciones de casos de uso (CU-01 a CU-12), convenciones de documentación y material para Word. |
| [`ea/`](ea/) | Modelos UML creados con **Enterprise Architect**. |
| [`MDS2/`](MDS2/) | Documentación de la materia **Metodologías de Desarrollo 2** (examen final). |

## Diagramas (`mermaid-live/`)

| Archivo | Descripción |
|---------|-------------|
| [`4.1.casos-uso-general.mmd`](mermaid-live/4.1.casos-uso-general.mmd) | Diagrama general de casos de uso por actor (Administrador, Coordinador, Profesor y Director). Fuente canónica. |
| [`7.1.clases-seguridad.mmd`](mermaid-live/7.1.clases-seguridad.mmd) | Diagrama de clases — módulo **Seguridad**: usuarios, roles/permisos (patrón **Composite**), login, 2FA, sesión. |
| [`7.2.clases-academico.mmd`](mermaid-live/7.2.clases-academico.mmd) | Diagrama de clases — módulo **Académico**: aulas, cursos, inscripciones y asistencia. |
| [`7.3.clases-tickets.mmd`](mermaid-live/7.3.clases-tickets.mmd) | Diagrama de clases — módulo **Tickets**: TicketBE y su cadena BLL → MPP → DAL + formularios. |
| [`7.4.clases-actividad-integridad.mmd`](mermaid-live/7.4.clases-actividad-integridad.mmd) | Diagrama de clases — módulos **Actividad e Integridad (DVH)**: bitácora de actividad y auditoría de integridad. |
| [`7.5.clases-infraestructura.mmd`](mermaid-live/7.5.clases-infraestructura.mmd) | Diagrama de clases — **Infraestructura y GUI base**: AccessDAL, ServiceLocator, sesión, preferencias/cultura, temas y formularios raíz. |
| [`8.er.mmd`](mermaid-live/8.er.mmd) | Modelo entidad-relación de la base de datos. |
| [`4.2.8.secuencia-crear-curso.mmd`](mermaid-live/4.2.8.secuencia-crear-curso.mmd) | Secuencia simple de crear curso (vista de alto nivel). |
| [`4.2.8.secuencia-crear-curso-completo.mmd`](mermaid-live/4.2.8.secuencia-crear-curso-completo.mmd) | Secuencia completa del login: Coordinador → CourseBLL → CourseMPP → AccessDAL → SQL Server. |
| [`4.3.8.secuencia-crear-aula.mmd`](mermaid-live/4.3.8.secuencia-crear-aula.mmd) | Secuencia simple de crear aula (vista de alto nivel). |
| [`4.3.8.secuencia-crear-aula-completo.mmd`](mermaid-live/4.3.8.secuencia-crear-aula-completo.mmd) | Secuencia completa de crear aula: Coordinador → ClassroomBLL → ClassroomMPP → AccessDAL → SQL Server. |
| [`4.4.8.secuencia-asistencia.mmd`](mermaid-live/4.4.8.secuencia-asistencia.mmd) | Secuencia simple de registro de asistencia (vista de alto nivel). |
| [`4.4.8.secuencia-asistencia-completo.mmd`](mermaid-live/4.4.8.secuencia-asistencia-completo.mmd) | Secuencia completa de asistencia: Profesor → EnrollmentBLL → EnrollmentMPP → AccessDAL → SQL Server. |
| [`4.5.8.secuencia-inscripcion.mmd`](mermaid-live/4.5.8.secuencia-inscripcion.mmd) | Secuencia simple de registrar inscripción (vista de alto nivel). |
| [`4.5.8.secuencia-inscripcion-completo.mmd`](mermaid-live/4.5.8.secuencia-inscripcion-completo.mmd) | Secuencia completa de inscripción: Coordinador → EnrollmentBLL → EnrollmentMPP → AccessDAL → SQL Server. |
| [`4.6.8.secuencia-tickets.mmd`](mermaid-live/4.6.8.secuencia-tickets.mmd) | Secuencia simple de crear/cerrar tickets (vista de alto nivel). |
| [`4.6.8.secuencia-tickets-completo.mmd`](mermaid-live/4.6.8.secuencia-tickets-completo.mmd) | Secuencia completa de tickets: Coordinador/Profesor → TicketBLL → TicketMPP → AccessDAL → SQL Server. |
| [`4.7.8.secuencia-gestionar-usuarios.mmd`](mermaid-live/4.7.8.secuencia-gestionar-usuarios.mmd) | Secuencia simple de gestionar usuarios (vista de alto nivel). |
| [`4.7.8.secuencia-gestionar-usuarios-completo.mmd`](mermaid-live/4.7.8.secuencia-gestionar-usuarios-completo.mmd) | Secuencia completa de usuarios: Administrador → UserBLL → UserMPP → AccessDAL → SQL Server (alta, edición, baja lógica y búsqueda). |
| [`4.8.8.secuencia-gestionar-roles.mmd`](mermaid-live/4.8.8.secuencia-gestionar-roles.mmd) | Secuencia simple de gestionar roles (vista de alto nivel). |
| [`4.8.8.secuencia-gestionar-roles-completo.mmd`](mermaid-live/4.8.8.secuencia-gestionar-roles-completo.mmd) | Secuencia completa de roles y permisos: Administrador → RoleBLL → RoleMPP → AccessDAL → SQL Server. |
| [`4.9.8.secuencia-evaluaciones.mmd`](mermaid-live/4.9.8.secuencia-evaluaciones.mmd) | Secuencia simple de registrar evaluación y notas (vista de alto nivel). |
| [`4.9.8.secuencia-evaluaciones-completo.mmd`](mermaid-live/4.9.8.secuencia-evaluaciones-completo.mmd) | Secuencia completa de evaluaciones: Profesor → EvaluationBLL → EvaluationMPP → AccessDAL → SQL Server (módulo a implementar). |
| [`4.10.8.secuencia-rendimiento.mmd`](mermaid-live/4.10.8.secuencia-rendimiento.mmd) | Secuencia simple de consultar rendimiento académico (vista de alto nivel). |
| [`4.10.8.secuencia-rendimiento-completo.mmd`](mermaid-live/4.10.8.secuencia-rendimiento-completo.mmd) | Secuencia completa de rendimiento: Coordinador/Director/Profesor → ReportBLL → ReportMPP → AccessDAL → SQL Server (módulo a implementar). |
| [`4.11.8.secuencia-informes.mmd`](mermaid-live/4.11.8.secuencia-informes.mmd) | Secuencia simple de generar informes personalizados (vista de alto nivel). |
| [`4.11.8.secuencia-informes-completo.mmd`](mermaid-live/4.11.8.secuencia-informes-completo.mmd) | Secuencia completa de informes: Coordinador/Director → ReportBLL → ReportMPP → AccessDAL → SQL Server (módulo a implementar). |
| [`6.1.8.secuencia-login.mmd`](mermaid-live/6.1.8.secuencia-login.mmd) | Secuencia simple del inicio de sesión (vista de alto nivel). |
| [`6.1.8.secuencia-login-completo.mmd`](mermaid-live/6.1.8.secuencia-login-completo.mmd) | Secuencia completa del login: AuthBLL → UserMPP → AccessDAL → SQL Server, ActivityBLL → ActivityMPP, PermissionBLL → RoleMPP, SessionManagerBLL, AppPreferencesBLL. |
| [`6.2.8.secuencia-logout.mmd`](mermaid-live/6.2.8.secuencia-logout.mmd) | Secuencia simple del cierre de sesión (vista de alto nivel). |
| [`6.2.8.secuencia-logout-completo.mmd`](mermaid-live/6.2.8.secuencia-logout-completo.mmd) | Secuencia completa del logout: AppPreferencesBLL (archivo), SessionManagerBLL (memoria), ActivityBLL → ActivityMPP → AccessDAL → SQL Server, CultureHelperBLL, ThemeHelper. |

Los archivos `.mmd` pueden visualizarse en **GitHub** (renderizado nativo de Mermaid), en el [Mermaid Live Editor](https://mermaid.live/) o con cualquier editor/visitor compatible.

## Modelos Enterprise Architect (`ea/`)

Carpeta destinada a los archivos de modelado UML creados con **Sparx Enterprise Architect** (casos de uso, clases, secuencia, etc.).

## Documentación de la materia (`MDS2/`)

| Archivo | Descripción |
|---------|-------------|
| [`MDS2 - Examen Final - Zubaray Dario.pdf`](MDS2/MDS2%20-%20Examen%20Final%20-%20Zubaray%20Dario.pdf) | Examen final de la cursada Metodologías de Desarrollo 2. |
