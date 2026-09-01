using System;
using System.Configuration;
using System.IO;
using System.Xml;
using Microsoft.Data.SqlClient;

namespace MPP.Tests.Setup
{
    public static class TestDatabaseHelper
    {
        public static string TestConnectionString
        {
            get
            {
                ConnectionStringSettings settings =
                    ConfigurationManager.ConnectionStrings["cadenaConexion"];

                if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                {
                    string connectionString = ReadConnectionStringFromConfigFile();
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        return connectionString;
                    }
                }

                if (settings != null && !string.IsNullOrEmpty(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }

                string fromEnv = Environment.GetEnvironmentVariable("TRABAJO_FINAL_TEST_CONNECTION");
                if (!string.IsNullOrEmpty(fromEnv))
                {
                    return fromEnv;
                }

                throw new InvalidOperationException(
                    "No se pudo resolver la cadena de conexión 'cadenaConexion'. " +
                    "Verifique el archivo MPP.Tests.dll.config o la variable de entorno TRABAJO_FINAL_TEST_CONNECTION.");
            }
        }

        private static string ReadConnectionStringFromConfigFile()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "MPP.Tests.dll.config");

            if (!File.Exists(configPath))
            {
                return null;
            }

            var doc = new XmlDocument();
            doc.Load(configPath);
            XmlNode node = doc.SelectSingleNode(
                "configuration/connectionStrings/add[@name='cadenaConexion']");

            return node?.Attributes?["connectionString"]?.Value;
        }

        private static string MasterConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder(TestConnectionString)
                {
                    InitialCatalog = "master"
                };
                return builder.ConnectionString;
            }
        }

        public static void EnsureDatabaseExists()
        {
            var builder = new SqlConnectionStringBuilder(TestConnectionString);
            string databaseName = builder.InitialCatalog;

            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    $"IF (DB_ID(N'{databaseName}') IS NULL) BEGIN CREATE DATABASE [{databaseName}]; END",
                    connection);
                cmd.ExecuteNonQuery();
            }
        }

        public static void CreateSchema()
        {
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();

                string sql = @"
                    IF OBJECT_ID('dbo.ActivityLogs', 'U') IS NOT NULL DROP TABLE dbo.ActivityLogs;
                    IF OBJECT_ID('dbo.RoleHierarchy', 'U') IS NOT NULL DROP TABLE dbo.RoleHierarchy;
                    IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
                    IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
                    IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL DROP TABLE dbo.Permissions;
                    IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;

                    CREATE TABLE [dbo].[Roles] (
                        [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name] NVARCHAR(100)  NOT NULL UNIQUE
                    );

                    CREATE TABLE [dbo].[Permissions] (
                        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name]        NVARCHAR(100)  NOT NULL UNIQUE,
                        [label]       NVARCHAR(100)  NOT NULL,
                        [description] NVARCHAR(256)  NULL,
                        [is_system]   BIT            NOT NULL DEFAULT 0
                    );

                    CREATE TABLE [dbo].[RolePermissions] (
                        [role_id]       INT NOT NULL,
                        [permission_id] INT NOT NULL,
                        PRIMARY KEY ([role_id], [permission_id]),
                        CONSTRAINT fk_rolepermissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[Roles]([id]),
                        CONSTRAINT fk_rolepermissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[Permissions]([id])
                    );

                    CREATE TABLE [dbo].[RoleHierarchy] (
                        [parent_role_id] INT NOT NULL,
                        [child_role_id]  INT NOT NULL,
                        PRIMARY KEY ([parent_role_id], [child_role_id]),
                        CONSTRAINT fk_rolehierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[Roles]([id]),
                        CONSTRAINT fk_rolehierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[Roles]([id])
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
                ";

                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public static void SeedTestData()
        {
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO [dbo].[Roles] (name) VALUES ('Admin'), ('Supervisor'), ('Operador');

                    INSERT INTO [dbo].[Permissions] (name, label, description, is_system) VALUES
                        ('FORM_USER_MGMT', 'Usuarios', 'Gestión de usuarios', 1),
                        ('FORM_ROLE_MGMT', 'Roles', 'Gestión de roles', 1),
                        ('FORM_COMPLAINTS', 'Reclamos', 'Gestión de reclamos', 0),
                        ('FORM_REPORTS', 'Reportes', 'Reportes', 0);

                    INSERT INTO [dbo].[RolePermissions] (role_id, permission_id) VALUES
                        (1, 1), (1, 2), (1, 3), (1, 4),
                        (2, 1), (2, 3),
                        (3, 4);

                    INSERT INTO [dbo].[RoleHierarchy] (parent_role_id, child_role_id) VALUES
                        (1, 2), (2, 3);

                    INSERT INTO [dbo].[Users]
                        (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                    VALUES
                        ('testuser',   'hashedpassword123', 1, 0, GETDATE(), GETDATE(), 'es',    'System', 1),
                        ('seconduser', 'hashedpassword456', 1, 0, GETDATE(), GETDATE(), 'en',    'Dark',   2),
                        ('thirduser',  'hashedpassword789', 0, 3, GETDATE(), GETDATE(), 'pt-BR', 'Light',  3);
                ";

                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public static void CleanDatabase()
        {
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();
                string sql = @"
                    DELETE FROM [dbo].[ActivityLogs];
                    DELETE FROM [dbo].[RolePermissions];
                    DELETE FROM [dbo].[RoleHierarchy];
                    DELETE FROM [dbo].[Users];
                    DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 0);";
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }
    }
}