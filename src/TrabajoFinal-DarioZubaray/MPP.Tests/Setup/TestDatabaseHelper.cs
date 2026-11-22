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
                    IF OBJECT_ID('dbo.grades', 'U') IS NOT NULL DROP TABLE dbo.grades;
                    IF OBJECT_ID('dbo.evaluations', 'U') IS NOT NULL DROP TABLE dbo.evaluations;
                    IF OBJECT_ID('dbo.tickets', 'U') IS NOT NULL DROP TABLE dbo.tickets;
                    IF OBJECT_ID('dbo.attendance', 'U') IS NOT NULL DROP TABLE dbo.attendance;
                    IF OBJECT_ID('dbo.course_students', 'U') IS NOT NULL DROP TABLE dbo.course_students;
                    IF OBJECT_ID('dbo.course_teachers', 'U') IS NOT NULL DROP TABLE dbo.course_teachers;
                    IF OBJECT_ID('dbo.courses', 'U') IS NOT NULL DROP TABLE dbo.courses;
                    IF OBJECT_ID('dbo.classrooms', 'U') IS NOT NULL DROP TABLE dbo.classrooms;
                    IF OBJECT_ID('dbo.activity_logs', 'U') IS NOT NULL DROP TABLE dbo.activity_logs;
                    IF OBJECT_ID('dbo.students', 'U') IS NOT NULL DROP TABLE dbo.students;
                    IF OBJECT_ID('dbo.role_hierarchy', 'U') IS NOT NULL DROP TABLE dbo.role_hierarchy;
                    IF OBJECT_ID('dbo.role_permissions', 'U') IS NOT NULL DROP TABLE dbo.role_permissions;
                    IF OBJECT_ID('dbo.schema_versions', 'U') IS NOT NULL DROP TABLE dbo.schema_versions;
                    IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users;
                    IF OBJECT_ID('dbo.permissions', 'U') IS NOT NULL DROP TABLE dbo.permissions;
                    IF OBJECT_ID('dbo.roles', 'U') IS NOT NULL DROP TABLE dbo.roles;

                    CREATE TABLE [dbo].[roles] (
                        [id]   INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name] NVARCHAR(100)  NOT NULL UNIQUE
                    );

                    CREATE TABLE [dbo].[permissions] (
                        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name]        NVARCHAR(100)  NOT NULL UNIQUE,
                        [label]       NVARCHAR(100)  NOT NULL,
                        [description] NVARCHAR(256)  NULL,
                        [is_system]   BIT            NOT NULL DEFAULT 0
                    );

                    CREATE TABLE [dbo].[role_permissions] (
                        [role_id]       INT NOT NULL,
                        [permission_id] INT NOT NULL,
                        PRIMARY KEY ([role_id], [permission_id]),
                        CONSTRAINT fk_role_permissions_roles       FOREIGN KEY ([role_id])       REFERENCES [dbo].[roles]([id]),
                        CONSTRAINT fk_role_permissions_permissions FOREIGN KEY ([permission_id]) REFERENCES [dbo].[permissions]([id])
                    );

                    CREATE TABLE [dbo].[role_hierarchy] (
                        [parent_role_id] INT NOT NULL,
                        [child_role_id]  INT NOT NULL,
                        PRIMARY KEY ([parent_role_id], [child_role_id]),
                        CONSTRAINT fk_role_hierarchy_parent FOREIGN KEY ([parent_role_id]) REFERENCES [dbo].[roles]([id]),
                        CONSTRAINT fk_role_hierarchy_child  FOREIGN KEY ([child_role_id])  REFERENCES [dbo].[roles]([id])
                    );

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
                        [dvh]           NVARCHAR(64)   NULL,
                        CONSTRAINT fk_users_roles FOREIGN KEY ([role_id]) REFERENCES [dbo].[roles]([id])
                    );
                    ALTER TABLE [dbo].[users] ADD CONSTRAINT uq_users_username UNIQUE ([user_name]);

                    CREATE TABLE [dbo].[students] (
                        [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [first_name]   NVARCHAR(100)  NOT NULL,
                        [last_name]    NVARCHAR(100)  NOT NULL,
                        [document]     NVARCHAR(20)   NOT NULL,
                        [email]        NVARCHAR(150)  NULL,
                        [phone]        NVARCHAR(50)   NULL,
                        [is_active]    BIT            NOT NULL DEFAULT 1,
                        [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                        [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
                    );
                    ALTER TABLE [dbo].[students] ADD CONSTRAINT uq_students_document UNIQUE ([document]);

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

                    CREATE TABLE [dbo].[classrooms] (
                        [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name]         NVARCHAR(100)  NOT NULL,
                        [capacity]     INT            NOT NULL,
                        [is_active]    BIT            NOT NULL DEFAULT 1,
                        [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                        [last_update]  DATETIME       NOT NULL DEFAULT GETDATE()
                    );

                    CREATE TABLE [dbo].[courses] (
                        [id]            INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [name]          NVARCHAR(200)  NOT NULL,
                        [description]   NVARCHAR(500)  NULL,
                        [start_date]    DATETIME       NOT NULL,
                        [end_date]      DATETIME       NOT NULL,
                        [classroom_id]  INT            NOT NULL,
                        [day_of_week]   INT            NULL,
                        [start_time]    TIME           NULL,
                        [end_time]      TIME           NULL,
                        [is_active]     BIT            NOT NULL DEFAULT 1,
                        [created_at]    DATETIME       NOT NULL DEFAULT GETDATE(),
                        [last_update]   DATETIME       NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT fk_courses_classrooms FOREIGN KEY ([classroom_id]) REFERENCES [dbo].[classrooms]([id])
                    );

                    CREATE TABLE [dbo].[course_teachers] (
                        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [course_id]    INT      NOT NULL,
                        [teacher_id]   INT      NOT NULL,
                        [is_active]    BIT      NOT NULL DEFAULT 1,
                        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT fk_course_teachers_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                        CONSTRAINT fk_course_teachers_users   FOREIGN KEY ([teacher_id]) REFERENCES [dbo].[users]([id])
                    );

                    CREATE TABLE [dbo].[course_students] (
                        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [course_id]    INT      NOT NULL,
                        [student_id]   INT      NOT NULL,
                        [is_active]    BIT      NOT NULL DEFAULT 1,
                        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT fk_course_students_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                        CONSTRAINT fk_course_students_students FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id])
                    );

                    CREATE TABLE [dbo].[attendance] (
                        [id]           INT      NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [course_id]    INT      NOT NULL,
                        [student_id]   INT      NOT NULL,
                        [date]         DATE     NOT NULL,
                        [is_present]   BIT      NOT NULL DEFAULT 0,
                        [created_at]   DATETIME NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT fk_attendance_courses FOREIGN KEY ([course_id])  REFERENCES [dbo].[courses]([id]),
                        CONSTRAINT fk_attendance_students FOREIGN KEY ([student_id]) REFERENCES [dbo].[students]([id]),
                        CONSTRAINT uq_attendance_unique  UNIQUE ([course_id], [student_id], [date])
                    );

                    CREATE TABLE [dbo].[evaluations] (
                        [id]           INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [course_id]    INT            NOT NULL,
                        [name]         NVARCHAR(200)  NOT NULL,
                        [date]         DATE           NOT NULL,
                        [created_by]   INT            NOT NULL,
                        [created_at]   DATETIME       NOT NULL DEFAULT GETDATE(),
                        [last_update]  DATETIME       NOT NULL DEFAULT GETDATE(),
                        [dvh]          NVARCHAR(64)   NULL,
                        CONSTRAINT fk_evaluations_courses FOREIGN KEY ([course_id]) REFERENCES [dbo].[courses]([id]),
                        CONSTRAINT fk_evaluations_users   FOREIGN KEY ([created_by]) REFERENCES [dbo].[users]([id])
                    );

                    CREATE TABLE [dbo].[grades] (
                        [id]             INT           NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [evaluation_id]  INT           NOT NULL,
                        [student_id]     INT           NOT NULL,
                        [grade]          DECIMAL(4,2)  NOT NULL,
                        [dvh]            NVARCHAR(64)  NULL,
                        CONSTRAINT fk_grades_evaluations FOREIGN KEY ([evaluation_id]) REFERENCES [dbo].[evaluations]([id]),
                        CONSTRAINT fk_grades_students    FOREIGN KEY ([student_id])    REFERENCES [dbo].[students]([id]),
                        CONSTRAINT uq_grades_evaluation_student UNIQUE ([evaluation_id], [student_id])
                    );

                    CREATE TABLE [dbo].[tickets] (
                        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [user_id]     INT            NOT NULL,
                        [subject]     NVARCHAR(200)  NOT NULL,
                        [description] NVARCHAR(MAX)  NOT NULL,
                        [is_closed]   BIT            NOT NULL DEFAULT 0,
                        [created_at]  DATETIME       NOT NULL DEFAULT GETDATE(),
                        [closed_at]   DATETIME       NULL,
                        [closed_by]   INT            NULL,
                        CONSTRAINT fk_tickets_user FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]),
                        CONSTRAINT fk_tickets_closed_by FOREIGN KEY ([closed_by]) REFERENCES [dbo].[users]([id])
                    );

                    CREATE TABLE [dbo].[schema_versions] (
                        [id]          INT            NOT NULL PRIMARY KEY IDENTITY(1,1),
                        [version]     NVARCHAR(20)   NOT NULL,
                        [script_name] NVARCHAR(200)  NOT NULL,
                        [applied_at]  DATETIME       NOT NULL DEFAULT GETDATE()
                    );
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
                    INSERT INTO [dbo].[roles] (name) VALUES ('Admin'), ('Supervisor'), ('Operador');

                    INSERT INTO [dbo].[permissions] (name, label, description, is_system) VALUES
                        ('FORM_USER_MGMT', 'Usuarios', 'Gestión de usuarios', 1),
                        ('FORM_ROLE_MGMT', 'Roles', 'Gestión de roles', 1),
                        ('FORM_STUDENT_MGMT', 'Alumnos', 'Gestión de alumnos', 0),
                        ('FORM_REPORTS', 'Reportes', 'Reportes', 0);

                    INSERT INTO [dbo].[role_permissions] (role_id, permission_id) VALUES
                        (1, 1), (1, 2), (1, 3), (1, 4),
                        (2, 1), (2, 3),
                        (3, 4);

                    INSERT INTO [dbo].[role_hierarchy] (parent_role_id, child_role_id) VALUES
                        (1, 2), (2, 3);

                    INSERT INTO [dbo].[users]
                        (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                    VALUES
                        ('testuser',   'hashedpassword123', 1, 0, GETDATE(), GETDATE(), 'es',    'System', 1),
                        ('seconduser', 'hashedpassword456', 1, 0, GETDATE(), GETDATE(), 'en',    'Dark',   2),
                        ('thirduser',  'hashedpassword789', 0, 3, GETDATE(), GETDATE(), 'pt-BR', 'Light',  3);

                    INSERT INTO [dbo].[students]
                        (first_name, last_name, document, email, phone, is_active, created_at, last_update)
                    VALUES
                        ('Juan',  'Pérez', 'DNI-10000001', 'juan.perez@test.com',  '11111111', 1, GETDATE(), GETDATE()),
                        ('María', 'Gómez', 'DNI-10000002', 'maria.gomez@test.com', '22222222', 1, GETDATE(), GETDATE());
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
                    DELETE FROM [dbo].[grades];
                    DELETE FROM [dbo].[evaluations];
                    DELETE FROM [dbo].[tickets];
                    DELETE FROM [dbo].[attendance];
                    DELETE FROM [dbo].[course_students];
                    DELETE FROM [dbo].[course_teachers];
                    DELETE FROM [dbo].[courses];
                    DELETE FROM [dbo].[classrooms];
                    DELETE FROM [dbo].[activity_logs];
                    DELETE FROM [dbo].[students];
                    DELETE FROM [dbo].[role_permissions];
                    DELETE FROM [dbo].[role_hierarchy];
                    DELETE FROM [dbo].[users];
                    DBCC CHECKIDENT ('[dbo].[users]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[students]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[classrooms]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[courses]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[course_teachers]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[course_students]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[attendance]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[tickets]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[evaluations]', RESEED, 0);
                    DBCC CHECKIDENT ('[dbo].[grades]', RESEED, 0);";
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }
    }
}