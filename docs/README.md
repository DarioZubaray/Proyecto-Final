# Documentación (`docs/`)

Esta carpeta reúne material de documentación del trabajo final: **diagramas** en formato Mermaid, modelos UML de **Enterprise Architect** y documentación de la cursada.

## Contenido

| Subcarpeta | Descripción |
|------------|-------------|
| [`mermaid-live/`](mermaid-live/) | Diagramas en formato [Mermaid](https://mermaid.js.org/): casos de uso, clases, entidad-relación y secuencias de login/logout. |
| [`ea/`](ea/) | Modelos UML creados con **Enterprise Architect**. |
| [`MDS2/`](MDS2/) | Documentación de la materia **Metodologías de Desarrollo 2** (examen final). |

## Diagramas (`mermaid-live/`)

| Archivo | Descripción |
|---------|-------------|
| [`4.1.casos-uso.mmd`](mermaid-live/4.1.casos-uso.mmd) | Casos de uso por actor (Coordinador, Profesor). |
| [`7.clases.mmd`](mermaid-live/7.clases.mmd) | Diagrama de clases con el patrón **Composite** de roles/permisos. |
| [`8.er.mmd`](mermaid-live/8.er.mmd) | Modelo entidad-relación de la base de datos. |
| [`4.2.8.secuencia-crear-curso.mmd`](mermaid-live/4.2.8.secuencia-crear-curso.mmd) | Secuencia simple de crear curso (vista de alto nivel). |
| [`4.2.8.secuencia-crear-curso-completo.mmd`](mermaid-live/4.2.8.secuencia-crear-curso-completo.mmd) | Secuencia completa del login: Coordinador → CourseBLL → CourseMPP → AccessDAL → SQL Server. |
| [`4.3.8.secuencia-crear-aula.mmd`](mermaid-live/4.3.8.secuencia-crear-aula.mmd) | Secuencia simple de crear aula (vista de alto nivel). |
| [`4.3.8.secuencia-crear-aula-completo.mmd`](mermaid-live/4.3.8.secuencia-crear-aula-completo.mmd) | Secuencia completa de crear aula: Coordinador → ClassroomBLL → ClassroomMPP → AccessDAL → SQL Server. |
| [`4.4.8.secuencia-asistencia.mmd`](mermaid-live/4.4.8.secuencia-asistencia.mmd) | Secuencia simple de registro de asistencia (vista de alto nivel). |
| [`4.4.8.secuencia-asistencia-completo.mmd`](mermaid-live/4.4.8.secuencia-asistencia-completo.mmd) | Secuencia completa de asistencia: Profesor → EnrollmentBLL → EnrollmentMPP → AccessDAL → SQL Server. |
| [`4.5.8.secuencia-inscripcion.mmd`](mermaid-live/4.5.8.secuencia-inscripcion.mmd) | Secuencia simple de registrar inscripción (vista de alto nivel). |
| [`4.5.8.secuencia-inscripcion-completo.mmd`](mermaid-live/4.5.8.secuencia-inscripcion-completo.mmd) | Secuencia completa de inscripción: Coordinador → EnrollmentBLL → EnrollmentMPP → AccessDAL → SQL Server. |
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
