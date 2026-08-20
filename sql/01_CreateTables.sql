use Trabajo_Final;

-- 1 Tabla de roles
CREATE TABLE [dbo].[Roles] (
    [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name] NVARCHAR(100)  NOT NULL UNIQUE
);

-- 2 Tabla de opciones de menú / permisos
CREATE TABLE [dbo].[MenuOptions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name]        NVARCHAR(100)  NOT NULL UNIQUE,
    [label]       NVARCHAR(100)  NOT NULL,
    [description] NVARCHAR(256)  NULL,
    [is_global]   BIT            NOT NULL DEFAULT 0
);

-- 3 Tabla intermedia Roles <-> MenuOptions (N:N)
CREATE TABLE [dbo].[RoleMenuOptions] (
    [role_id]        INT NOT NULL,
    [menu_option_id] INT NOT NULL,
    PRIMARY KEY ([role_id], [menu_option_id]),
    CONSTRAINT fk_rolemenuoptions_roles       FOREIGN KEY ([role_id])        REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolemenuoptions_menuoptions FOREIGN KEY ([menu_option_id]) REFERENCES [dbo].[MenuOptions]([id])
);

-- 4 Tabla de usuarios
CREATE TABLE [dbo].[Users] (
    [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [user_name]     NVARCHAR(100)  NOT NULL,
    [password_hash] NVARCHAR(256)  NOT NULL,
    [is_active]     BIT            NOT NULL DEFAULT 1,
    [retries_count] INT            NOT NULL DEFAULT 0,
    [last_update]   DATETIME       NOT NULL,
    [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
    [role_id]       INT            NULL,

    CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[Roles]([id])
);
ALTER TABLE [dbo].[Users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);
