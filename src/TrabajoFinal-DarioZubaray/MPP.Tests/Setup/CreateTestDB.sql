-- Script para crear la base de datos de test
-- Ejecutar una sola vez sobre SQL Server Express

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Trabajo_Final_Test')
BEGIN
    CREATE DATABASE Trabajo_Final_Test;
END
GO

USE Trabajo_Final_Test;
GO

-- Eliminar tablas si existen (para recrear limpio)
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL DROP TABLE dbo.Permissions;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

-- Crear tablas
CREATE TABLE [dbo].[Roles] (
    [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name] NVARCHAR(100)  NOT NULL UNIQUE
);

CREATE TABLE [dbo].[Permissions] (
    [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
    [name]        NVARCHAR(100)  NOT NULL UNIQUE,
    [label]       NVARCHAR(100)  NOT NULL,
    [description] NVARCHAR(256)  NULL
);

CREATE TABLE [dbo].[RolePermissions] (
    [role_id]       INT NOT NULL,
    [permission_id] INT NOT NULL,
    PRIMARY KEY ([role_id], [permission_id]),
    CONSTRAINT fk_rolepermissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[Roles]([id]),
    CONSTRAINT fk_rolepermissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[Permissions]([id])
);

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
GO

-- Datos seed para tests
INSERT INTO [dbo].[Roles] (name) VALUES ('Admin'), ('Supervisor'), ('Operador');

INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, language, role_id, theme)
VALUES ('testuser', '$2a$11$abcdefghijklmnopqrstuuABCDEFGHIJKLMNOPQRSTUVWXYZ12345678', 1, 0, GETDATE(), GETDATE(), 'es', 1, 'System');
GO
