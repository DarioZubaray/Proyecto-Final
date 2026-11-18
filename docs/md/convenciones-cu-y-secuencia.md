# Convenciones — Especificaciones de Caso de Uso y diagramas de secuencia

Convención de trabajo para las especificaciones (`docs/md/`) y los diagramas de
secuencia (`docs/mermaid-live/`). Surge de las correcciones de las entregas 1ra,
2da y 3ra.

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

### Participantes — instancias de clases, no capas

Corrección de la 3ra entrega: *"los diagramas de secuencia muestran capas pero no
instancian ni muestran clases u objetos"*. Cada lifeline es **un objeto de una clase
concreta** de la solución, rotulado con notación UML `objeto : Clase`:

```
actor <Rol>
participant frm as "frm : <FormClass>"
participant <alias> as "<alias> : <Class>"
participant dal as "dal : AccessDAL"
participant bd as "Base de Datos"
```

- `<Rol>` es el rol del actor principal (`actor Profesor`, `actor Coordinador`, etc.).
  Si el caso de uso tiene **más de un actor principal** se usa `actor Usuario`.
- `<alias>` es el nombre de instancia en camelCase tal como aparece en el código:
  `frm : CourseForm`, `courseBLL : CourseBLL`, `courseMPP : CourseMPP`,
  `course : CourseBE`, `dal : AccessDAL`.
- **Un lifeline por instancia realmente usada en el diagrama.** Si el formulario tiene
  varios BLL (p. ej. `CourseForm` con `_courseBLL`, `_classroomBLL` y `_userBLL`), el
  diagrama los muestra a los tres y cada llamada recorre la cadena real de su clase
  (`classroomBLL → classroomMPP → dal → Base de Datos`).
- **Creación de objetos**: cuando el código crea el BE con `new …()` (altas), el
  lifeline se declara en ese punto con `create participant` y **acto seguido** el
  mensaje de creación (mermaid lo exige):

```
create participant course as "course : CourseBE"
frm->>course: new CourseBE()
```

  Si el objeto ya existe al iniciar el flujo (ediciones, entidades cargadas), se
  declara arriba con su rotulación `objeto : Clase`. Opcionalmente se muestra la
  adquisición real del BLL en el constructor del formulario:
  `frm->>frm: _courseBLL = ServiceLocatorBLL.CreateCourseBLL()`.
- Se elimina la nota genérica *"…transportan los datos entre capas"*.

### Mensajes

- **Cada mensaje es una invocación real del código**: nombre de método tal como existe
  en la solución, con parámetros y tipo de retorno. Ejemplos reales:
  `Login(userName: string, password: string): LoginResultBE`,
  `GetByUserName(userName: string): UserBE`, `Save(course: CourseBE): bool`,
  `ReadScalar(consulta: string, parameters: SqlParameter[]): int`.
- La cadena debe **replicar la llamada real**:
  `frm → <xBLL> → <xMPP> → dal → Base de Datos`,
  y las respuestas en sentido contrario hasta el usuario.
- **Sin SQL**: ninguna sentencia (`SELECT`, `INSERT`, `UPDATE`…) en los mensajes.
  `dal` expone operaciones (`Read`, `ReadScalar`, `Save`), no consultas.
- **Sin valores de instancia**: se pasa la entidad (`CourseBE`), no
  `CourseBE{Name="Programación I"}` ni ids concretos.
- **Sin mensajes "verificar"/"validar"** como paso propio:
  - la validación de pantalla se dibuja en el `alt` que devuelve el error a `frm`
    (coincide con `ValidateInputs()` de `CourseForm`, `ClassroomForm`, `TicketForm`,
    `UserForm`);
  - la regla de negocio se dibuja como rechazo de negocio (ver abajo).
- **El actor solo conversa con `frm`**; el ping-pong usuario ↔ sistema se respeta
  arriba (actor ↔ frm) y hacia abajo son invocaciones entre instancias.

### Solo cuestiones de negocio (3ra entrega)

Los flujos alternativos del DSD describen **reglas y validaciones de negocio**.
Está prohibido dibujar ramas o mensajes técnicos:

- *"Error al guardar: \<detalle\>"*, *"Excepción de SQL Server"*,
  *"La base de datos rechaza la operación"*, *"Base de datos no disponible (DB-001)"*,
  `throw InvalidOperationException` / cualquier `Exception`, cadenas de prueba de
  conexión (`TestConnection`).
- La regla de negocio se pinta como **rechazo de negocio** con el mensaje que ve el
  usuario. En el código esa regla se lanza como `InvalidOperationException`, pero el
  diagrama solo pinta el mensaje de negocio:

```
alt El aula ya está ocupada en ese día y horario
    bll-->>frm: Rechaza: "El aula ya está ocupada en ese horario."
    frm-->>Usuario: Muestra "El aula ya está ocupada en ese horario."
else Aula libre
    …
```

- Se conservan los códigos de negocio (`BIZ-*`, `AUTH-*`, `VAL-*`, `INT-*`,
  `FORM_*`) porque son parte de la regla que el usuario ve.

### DSS vs DSD

| | DSS (simple) | DSD (detallado) |
| :--- | :--- | :--- |
| Participantes | los mismos (instancias `objeto : Clase`) | los mismos (instancias `objeto : Clase`) |
| Contenido | solo el flujo básico del CU | flujo básico + flujos alternativos de negocio (sin ramas técnicas) |
| Nombre de archivo | `X.Y.8.secuencia-<slug>.mmd` | `X.Y.8.secuencia-<slug>-completo.mmd` |

### Sincronización

- El bloque ` ```mermaid ` del `.md` y el `.mmd` del mismo CU deben ser **idénticos**.
  Para sincronizar un bloque se usa `sync-block.ps1 -Md <md> -Mmd <mmd> -Index <n>`
  (`n` = número de bloque ` ```mermaid ` contando desde 1 en el `.md`).
- Antes de regenerar los PNG se contrasta cada diagrama contra el código de la
  pantalla y del `BLL` correspondientes (que los métodos invocados existan y estén en
  el orden real).
- Los PNG se regeneran con
  `mmdc -i <archivo.mmd> -o <archivo.png> -p puppeteer-config.json -s 3`
  (desde `docs/mermaid-live/`).
