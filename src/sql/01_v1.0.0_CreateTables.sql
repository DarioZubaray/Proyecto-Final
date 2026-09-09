use Trabajo_Final;

-- =============================================
-- CREATE v1.0.0: Esquema completo de tablas
-- =============================================

-- 1. Roles
CREATE TABLE [dbo].[Roles] (
    [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name] NVARCHAR(100)  NOT NULL UNIQUE
);

-- 2. Permisos (cada permiso = 1 formulario)
CREATE TABLE [dbo].[Permissions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name]        NVARCHAR(100)  NOT NULL UNIQUE,
    [label]       NVARCHAR(100)  NOT NULL,
    [description] NVARCHAR(256)  NULL,
    [is_system]   BIT            NOT NULL DEFAULT 0
);

-- 3. Relación Roles <-> Permisos (N:N)
CREATE TABLE [dbo].[RolePermissions] (
    [role_id]       INT NOT NULL,
    [permission_id] INT NOT NULL,
    PRIMARY KEY ([role_id], [permission_id]),
    CONSTRAINT fk_rolepermissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolepermissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[Permissions]([id])
);

-- 4. Jerarquía de roles (padre -> hijo, para Composite)
CREATE TABLE [dbo].[RoleHierarchy] (
    [parent_role_id] INT NOT NULL,
    [child_role_id]  INT NOT NULL,
    PRIMARY KEY ([parent_role_id], [child_role_id]),
    CONSTRAINT fk_rolehierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolehierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[Roles]([id])
);

-- 5. Usuarios
CREATE TABLE [dbo].[Users] (
    [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [user_name]     NVARCHAR(100)  NOT NULL,
    [password_hash] NVARCHAR(256)  NOT NULL,
    [is_active]     BIT            NOT NULL DEFAULT 1,
    [retries_count] INT            NOT NULL DEFAULT 0,
    [last_update]   DATETIME       NOT NULL,
    [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
    [role_id]       INT            NULL,
    [language]      NVARCHAR(10)   NOT NULL DEFAULT 'es',
    [theme]         NVARCHAR(20)   NOT NULL DEFAULT 'System',
    CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[Roles]([id])
);
ALTER TABLE [dbo].[Users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);

-- 6. Bitácora de actividad (Historial de Actividad) por usuario
CREATE TABLE [dbo].[ActivityLogs] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [user_id]     INT            NOT NULL,
    [action]      NVARCHAR(64)   NOT NULL,
    [form_name]   NVARCHAR(100)  NULL,
    [description] NVARCHAR(256)  NULL,
    [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_activitylogs_users FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users]([id])
);
CREATE INDEX ix_activitylogs_user_created ON [dbo].[ActivityLogs] ([user_id], [created_at] DESC);

-- 7. Tabla de versiones de migración
CREATE TABLE [dbo].[SchemaVersions] (
    [Id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Version]     NVARCHAR(20)   NOT NULL,
    [ScriptName]  NVARCHAR(200)  NOT NULL,
    [AppliedAt]   DATETIME       NOT NULL DEFAULT GETDATE()
);

-- 8. Aulas
CREATE TABLE [dbo].[Aulas] (
    [Id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Nombre]      NVARCHAR(100)  NOT NULL,
    [Capacidad]   INT            NOT NULL,
    [Is_Active]   BIT            NOT NULL DEFAULT 1,
    [Created_At]  DATETIME       NOT NULL DEFAULT GETDATE(),
    [Last_Update] DATETIME       NOT NULL DEFAULT GETDATE()
);

-- 9. Cursos
CREATE TABLE [dbo].[Cursos] (
    [Id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Nombre]        NVARCHAR(200)  NOT NULL,
    [Descripcion]   NVARCHAR(500)  NULL,
    [Fecha_Inicio]  DATETIME       NOT NULL,
    [Fecha_Fin]     DATETIME       NOT NULL,
    [Aula_Id]       INT            NULL,
    [Is_Active]     BIT            NOT NULL DEFAULT 1,
    [Created_At]    DATETIME       NOT NULL DEFAULT GETDATE(),
    [Last_Update]   DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_cursos_aulas FOREIGN KEY ([Aula_Id]) REFERENCES [dbo].[Aulas]([Id])
);

-- 10. CursoDocentes (N:N)
CREATE TABLE [dbo].[CursoDocentes] (
    [Id]          INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Curso_Id]    INT      NOT NULL,
    [Docente_Id]  INT      NOT NULL,
    [Is_Active]   BIT      NOT NULL DEFAULT 1,
    [Created_At]  DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_cursodocentes_cursos   FOREIGN KEY ([Curso_Id])   REFERENCES [dbo].[Cursos]([Id]),
    CONSTRAINT fk_cursodocentes_users    FOREIGN KEY ([Docente_Id]) REFERENCES [dbo].[Users]([Id])
);
