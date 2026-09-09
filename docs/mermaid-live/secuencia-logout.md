```mermaid
sequenceDiagram
    actor User
    participant UI as :UI
    participant Auth as :Servicio de Autenticación

    User->>UI: Cerrar sesión()
    UI->>Auth: Remover sesión()
    Auth-->>UI: ok
    UI-->>User: ok
```
