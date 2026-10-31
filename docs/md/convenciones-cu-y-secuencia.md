# Convenciones — Especificaciones de Caso de Uso y diagramas de secuencia

Convención de trabajo para las especificaciones (`docs/md/`) y los diagramas de
secuencia (`docs/mermaid-live/`). Surge de las correcciones de la 1ra y 2da entrega.

## 1. Descripción analítica del Caso de Uso (4.x.6)

El flujo es un **ping-pong estricto entre el usuario y el sistema**:

1. **Cada paso es o una acción del usuario o la respuesta del sistema.** No existen
   pasos de acciones internas del sistema.
2. Prohibido: *"El sistema valida…"*, *"El sistema verifica…"*,
   *"El sistema consulta la base de datos…"*, *"El sistema persiste…"*.
   El usuario no se entera de qué hace el sistema por dentro.
3. **El paso 1 siempre es del usuario** (abrir el formulario, elegir una opción del menú).
4. **Las validaciones no son pasos.** Solo aparecen como la *respuesta* del sistema en un
   flujo alternativo, anclado al paso del usuario que las dispara
   (p. ej. `5.a` sobre el paso 5 "Guardar").
5. **Campos.** Se declara primero la lista de campos del formulario (tabla
   Campo / Tipo / Obligatorio / Validación) y después, en el flujo básico, se
   desglosa **un paso por campo** del lado del usuario
   (*"El usuario completa el campo \"Nombre\""*). Completar todos los campos juntos
   también es un ping válido, pero se usa el desglose por campo.
6. **Una respuesta del sistema = un paso** (mensaje, grilla, confirmación de alta).
7. Los flujos alternativos se numeran con la letra del paso que el usuario ejecuta
   (`5.a`, `5.b`…) y siempre cierran con el usuario reintentando o cancelando.

## 2. Diagramas de secuencia (DSS y DSD) — sección 4.x.8

### Participantes

Siempre los mismos, en este orden, y **son capas del proyecto, no clases** (por eso no
se les antepone `:`):

```
actor <Rol>
GUI          ← capa de presentación (formularios WinForms)
BE           ← capa de entidades de negocio (lifeline pasiva)
BLL          ← capa de lógica de negocio
MPP          ← capa de mapeo/persistencia
DAL          ← capa de acceso a datos (AccessDAL)
Base de Datos ← SQL Server
```

- `<Rol>` es el rol del actor principal: `actor Profesor`, `actor Coordinador`, etc.
  Si el caso de uso tiene **más de un actor principal** (p. ej. Coordinador y Profesor)
  se usa `actor Usuario`.

### Mensajes

- **Cada mensaje es una invocación real del código**: nombre de método tal como existe
  en la solución, con parámetros y tipo de retorno. Ejemplos reales:
  `Ingresar(userName: string, password: string): LoginResultBE`,
  `GetByUserName(userName: string): UserBE`, `Save(course: CourseBE): bool`,
  `ReadScalar(consulta: string, parameters: SqlParameter[]): int`.
- La cadena debe **replicar la llamada real**: `GUI → BLL → MPP → DAL → Base de Datos`,
  y las respuestas en sentido contrario hasta el usuario.
- **Sin SQL**: ninguna sentencia (`SELECT`, `INSERT`, `UPDATE`…) en los mensajes.
  La capa `DAL` expone operaciones (`Read`, `ReadScalar`, `Save`), no consultas.
- **Sin valores de instancia**: se pasa la entidad (`CourseBE`), no
  `CourseBE{Name="Programación I"}` ni ids concretos.
- **Sin mensajes "verificar"/"validar"** como paso propio:
  - la validación de pantalla se dibuja en el `alt` que devuelve el error a la `GUI`
    (coincide con `ValidateInputs()` de `CourseForm`, `ClassroomForm`, `TicketForm`, `UserForm`);
  - la regla de negocio se dibuja en el `alt` de la excepción que la `GUI` muestra al
    usuario (coincide con los `InvalidOperationException` de `CourseBLL`,
    `EnrollmentBLL`, `TicketBLL`).
- **El actor solo conversa con la `GUI`**; el ping-pong usuario ↔ sistema se respeta
  arriba (actor ↔ GUI) y hacia abajo son invocaciones entre capas.

### DSS vs DSD

| | DSS (simple) | DSD (detallado) |
| :--- | :--- | :--- |
| Participantes | los mismos 7 | los mismos 7 |
| Contenido | solo el flujo básico del CU | flujo básico + flujos alternativos + manejo de errores |
| Nombre de archivo | `X.Y.8.secuencia-<slug>.mmd` | `X.Y.8.secuencia-<slug>-completo.mmd` |

### Sincronización

- El bloque ` ```mermaid ` del `.md` y el `.mmd` del mismo CU deben ser **idénticos**.
- Antes de regenerar los PNG se contrasta cada diagrama contra el código de la
  pantalla y del `BLL` correspondientes (que los métodos invocados existan y estén en
  el orden real).
