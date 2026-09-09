use Trabajo_Final;

-- =============================================
-- CREATE: Esquema completo de tablas
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
