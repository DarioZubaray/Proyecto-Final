using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace DAL
{
    /// <summary>
    /// Lee y persiste la cadena de conexion "cadenaConexion" en el archivo
    /// de configuracion de la aplicacion (TrabajoFinal-DarioZubaray.dll.config),
    /// el mismo archivo que escribe el instalador de Inno Setup.
    /// </summary>
    public static class ConnectionConfigDAL
    {
        #region Propiedades
        public const string ConnectionName = "cadenaConexion";
        private const int TestTimeoutSeconds = 4;
        #endregion

        #region Lectura
        public static string CurrentConnectionString
        {
            get
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionName];
                return settings?.ConnectionString;
            }
        }

        public static void GetCurrent(out string server, out string database)
        {
            server = string.Empty;
            database = string.Empty;

            string connectionString = CurrentConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                server = builder.DataSource;
                database = builder.InitialCatalog;
            }
            catch
            {
            }
        }
        #endregion

        #region Construccion y prueba
        public static string Build(string server, string database)
        {
            return "Server=" + server.Trim()
                 + ";Database=" + database.Trim()
                 + ";Integrated Security=True;Encrypt=False;";
        }

        public static bool Test(string connectionString)
        {
            return TestError(connectionString) == null;
        }

        /// <summary>
        /// Prueba la conexion y devuelve null si fue exitosa, o el mensaje de error.
        /// </summary>
        public static string TestError(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return "Cadena de conexion vacia.";
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = TestTimeoutSeconds
                };

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Lista las bases de datos disponibles en el servidor. Devuelve una
        /// lista vacia si el servidor no responde.
        /// </summary>
        public static List<string> ListDatabases(string server)
        {
            var databases = new List<string>();

            if (string.IsNullOrWhiteSpace(server))
            {
                return databases;
            }

            try
            {
                using (var connection = new SqlConnection(Build(server, "master")))
                {
                    connection.Open();

                    using (var command = new SqlCommand(
                        "SELECT name FROM sys.databases WHERE state_desc = 'ONLINE' ORDER BY name",
                        connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                databases.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch
            {
            }

            return databases;
        }
        #endregion

        #region Escritura
        /// <summary>
        /// Persiste la cadena de conexion en el archivo de configuracion de la
        /// aplicacion y fuerza a ConfigurationManager a releerla.
        /// </summary>
        public static void Save(string server, string database)
        {
            string connectionString = Build(server, database);

            Configuration config = ConfigurationManager.OpenExeConfiguration(
                ConfigurationUserLevel.None);

            ConnectionStringSettings settings =
                config.ConnectionStrings.ConnectionStrings[ConnectionName];

            if (settings == null)
            {
                config.ConnectionStrings.ConnectionStrings.Add(
                    new ConnectionStringSettings(ConnectionName, connectionString));
            }
            else
            {
                settings.ConnectionString = connectionString;
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");

            string persisted = CurrentConnectionString;

            if (!string.Equals(persisted, connectionString, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "No se pudo actualizar el archivo de configuracion de conexion.");
            }
        }
        #endregion
    }
}
