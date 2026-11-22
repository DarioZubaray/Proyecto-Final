# Documentación (`docs/`)

Esta carpeta reúne material de documentación del trabajo final: **diagramas** en formato Mermaid, especificaciones de casos de uso y material para Word.

## Contenido

| Subcarpeta / archivo | Descripción |
|----------------------|-------------|
| [`mermaid-live/`](mermaid-live/) | Diagramas en formato [Mermaid](https://mermaid.js.org/) con sus PNG renderizados. |
| [`md/`](md/) | Especificaciones de casos de uso (CU-01 a CU-21), convenciones de documentación y material para Word. |
| [`drawio/`](drawio/) | Imágenes exportadas de los diagramas de actividad de la sección 3.1.3. |
| [`TP Final - Analista Programador - Dario Zubaray.docx`](TP%20Final%20-%20Analista%20Programador%20-%20Dario%20Zubaray.docx) | Documento principal de la entrega. |

## Diagramas (`mermaid-live/`)

Organizados por sección del documento. Cada `.mmd` tiene su `.png` renderizado al lado (relación 1:1):

| Carpeta | Contenido |
|---------|-----------|
| [`3. diagramas-actividad/`](mermaid-live/3.%20diagramas-actividad/) | Diagramas de proceso/actividad (sección 3.1.3). |
| [`4. diagramas-caso-uso/`](mermaid-live/4.%20diagramas-caso-uso/) | Casos de uso: diagrama general (`4.1.casos-uso-general`), diagrama por CU (`4.4.0` a `4.21.0`) y diagramas de secuencia por CU (`4.x.8.secuencia-*`, versión simple y `-completo`). |
| [`6. diagramas-cu-login-logout/`](mermaid-live/6.%20diagramas-cu-login-logout/) | Secuencias de CU-01 Login y CU-02 Logout. |
| [`7. diagramas-clases/`](mermaid-live/7.%20diagramas-clases/) | Diagramas de clases por módulo (`7.1` a `7.5`). |
| [`8. diagramas-entidad-relacion/`](mermaid-live/8.%20diagramas-entidad-relacion/) | Modelo entidad-relación (`8.er`). |

### Render de PNGs

```powershell
# un archivo
mmdc -i "<archivo>.mmd" -o "<archivo>.png" -p puppeteer-config.json -s 3
# todos los .mmd (recursivo)
.\generar-pngs.ps1
```

Requiere [`@mermaid-js/mermaid-cli`](https://github.com/mermaid-js/mermaid-cli) **12.x** (los diagramas de casos de uso usan la clave `usecase-beta`, introducida en Mermaid 12.0.0).

Los archivos `.mmd` pueden visualizarse en **GitHub** (renderizado nativo de Mermaid), en el [Mermaid Live Editor](https://mermaid.live/) o con cualquier editor/visitor compatible.
