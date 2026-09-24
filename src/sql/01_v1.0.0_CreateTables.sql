USE DarioZubaray_TF;

-- =============================================
-- CREATE v1.0.0: Esquema base de tablas
-- =============================================

-- 1. Roles
CREATE TABLE [dbo].[roles] (
    [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name] NVARCHAR(100)  NOT NULL UNIQUE
);

-- 2. Permisos
CREATE TABLE [dbo].[permissions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name]        NVARCHAR(100)  NOT NULL UNIQUE,
    [label]       NVARCHAR(100)  NOT NULL,
    [description] NVARCHAR(256)  NULL,
    [is_system]   BIT            NOT NULL DEFAULT 0
);

-- 3. Relacion Roles <-> Permisos (N:N)
CREATE TABLE [dbo].[role_permissions] (
    [role_id]       INT NOT NULL,
    [permission_id] INT NOT NULL,
    PRIMARY KEY ([role_id], [permission_id]),
    CONSTRAINT fk_role_permissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[roles]([id]),
    CONSTRAINT fk_role_permissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[permissions]([id])
);

-- 4. Jerarquia de roles
CREATE TABLE [dbo].[role_hierarchy] (
    [parent_role_id] INT NOT NULL,
    [child_role_id]  INT NOT NULL,
    PRIMARY KEY ([parent_role_id], [child_role_id]),
    CONSTRAINT fk_role_hierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[roles]([id]),
    CONSTRAINT fk_role_hierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[roles]([id])
);

-- 5. Usuarios
CREATE TABLE [dbo].[users] (
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
    CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[roles]([id])
);
ALTER TABLE [dbo].[users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);

-- 6. Bitacora de actividad
CREATE TABLE [dbo].[activity_logs] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [user_id]     INT            NOT NULL,
    [action]      NVARCHAR(64)   NOT NULL,
    [form_name]   NVARCHAR(100)  NULL,
    [description] NVARCHAR(256)  NULL,
    [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_activity_logs_users FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
);
CREATE INDEX ix_activity_logs_user_created ON [dbo].[activity_logs] ([user_id], [created_at] DESC);

-- 7. Tabla de versiones de migracion
CREATE TABLE [dbo].[schema_versions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [version]     NVARCHAR(20)   NOT NULL,
    [script_name] NVARCHAR(200)  NOT NULL,
    [applied_at]  DATETIME       NOT NULL DEFAULT GETDATE()
);
