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
| [`casos-uso.mmd`](mermaid-live/casos-uso.mmd) | Casos de uso por actor (Admin, Profesor, Alumno). |
| [`clases.mmd`](mermaid-live/clases.mmd) | Diagrama de clases con el patrón **Composite** de roles/permisos. |
| [`er.mmd`](mermaid-live/er.mmd) | Modelo entidad-relación de la base de datos. |
| [`secuencia-login.mmd`](mermaid-live/secuencia-login.mmd) | Secuencia simple del inicio de sesión (vista de alto nivel). |
| [`secuencia-login-completo.mmd`](mermaid-live/secuencia-login-completo.mmd) | Secuencia completa del login: AuthBLL → UserMPP → AccessDAL → SQL Server, ActivityBLL → ActivityMPP, PermissionBLL → RoleMPP, SessionManagerBLL, AppPreferencesBLL. |
| [`secuencia-logout.mmd`](mermaid-live/secuencia-logout.mmd) | Secuencia simple del cierre de sesión (vista de alto nivel). |
| [`secuencia-logout-completo.mmd`](mermaid-live/secuencia-logout-completo.mmd) | Secuencia completa del logout: AppPreferencesBLL (archivo), SessionManagerBLL (memoria), ActivityBLL → ActivityMPP → AccessDAL → SQL Server, CultureHelperBLL, ThemeHelper. |

Los archivos `.mmd` pueden visualizarse en **GitHub** (renderizado nativo de Mermaid), en el [Mermaid Live Editor](https://mermaid.live/) o con cualquier editor/visitor compatible.

## Modelos Enterprise Architect (`ea/`)

Carpeta destinada a los archivos de modelado UML creados con **Sparx Enterprise Architect** (casos de uso, clases, secuencia, etc.).

## Documentación de la materia (`MDS2/`)

| Archivo | Descripción |
|---------|-------------|
| [`MDS2 - Examen Final - Zubaray Dario.pdf`](MDS2/MDS2%20-%20Examen%20Final%20-%20Zubaray%20Dario.pdf) | Examen final de la cursada Metodologías de Desarrollo 2. |
