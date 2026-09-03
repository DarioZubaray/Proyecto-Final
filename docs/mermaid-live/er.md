```mermaid
erDiagram
    USERS {
        int id PK
        string user_name UK
        string password_hash
        bool is_active
        int retries_count
        datetime last_update
        datetime created_at
        int role_id FK
        string language
        string theme
    }
    ROLES {
        int id PK
        string name UK
    }
    PERMISSIONS {
        int id PK
        string name UK
        string label
        string description
        bool is_system
    }
    ROLE_PERMISSIONS {
        int role_id FK
        int permission_id FK
    }
    ROLE_HIERARCHY {
        int parent_role_id FK
        int child_role_id FK
    }
    ACTIVITY_LOGS {
        int id PK
        int user_id FK
        string action
        string form_name
        string description
        datetime created_at
    }

    USERS }o--|| ROLES : "role_id"
    ROLES ||--o{ ROLE_PERMISSIONS : "permisos"
    PERMISSIONS ||--o{ ROLE_PERMISSIONS : "asignado a"
    ROLES ||--o{ ROLE_HIERARCHY : "padre"
    ROLES ||--o{ ROLE_HIERARCHY : "hijo"
    USERS ||--o{ ACTIVITY_LOGS : "bitácora"
```