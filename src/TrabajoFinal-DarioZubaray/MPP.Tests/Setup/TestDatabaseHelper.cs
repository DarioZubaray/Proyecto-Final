using System;
using System.Configuration;
using System.Data.SqlClient;

namespace MPP.Tests.Setup
{
    public static class TestDatabaseHelper
    {
        private static string MasterConnectionString => "Server=PAPI-RYZEN3\\SQLEXPRESS;Database=master;Integrated Security=True;";

        public static string TestConnectionString => "Server=PAPI-RYZEN3\\SQLEXPRESS;Database=Trabajo_Final_Test;Integrated Security=True;";

        public static void EnsureDatabaseExists()
        {
            using (var connection = new SqlConnection(MasterConnectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(
                    "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Trabajo_Final_Test') " +
                    "CREATE DATABASE Trabajo_Final_Test", connection);
                cmd.ExecuteNonQuery();
            }
        }

        public static void CreateSchema()
        {
            using (var connection = new SqlConnection(TestConnectionString))
            {
                connection.Open();

                string sql = @"
                    IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
                    IF OBJECT_ID('dbo.RolePermissions', 'U') IS NOT NULL DROP TABLE dbo.RolePermissions;
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
                        [description] NVARCHAR(256)  NULL
                    );

                    CREATE TABLE [dbo].[RolePermissions] (
                        [role_id]       INT NOT NULL,
                        [permission_id] INT NOT NULL,
                        PRIMARY KEY ([role_id], [permission_id])
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
                        CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[Roles]([id])
                    );
                    ALTER TABLE [dbo].[Users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);
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
                    INSERT INTO [dbo].[Users] (user_name, password_hash, is_active, retries_count, last_update, created_at, language, role_id)
                    VALUES ('testuser', 'hashedpassword123', 1, 0, GETDATE(), GETDATE(), 'es', 1);
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
                var cmd = new SqlCommand("DELETE FROM [dbo].[Users]; DBCC CHECKIDENT ('[dbo].[Users]', RESEED, 0);", connection);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
