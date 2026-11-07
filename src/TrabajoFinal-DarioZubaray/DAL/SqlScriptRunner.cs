using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class SqlScriptRunner
    {
        #region Propiedades
        private readonly string _connectionString;
        #endregion

        #region Constructor
        public SqlScriptRunner()
            : this(null)
        {
        }

        public SqlScriptRunner(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = ConfigurationManager
                    .ConnectionStrings["cadenaConexion"].ConnectionString;
            }

            _connectionString = connectionString;
        }
        #endregion

        #region Métodos
        /// <summary>
        /// Quita las sentencias USE (la base destino la define la cadena
        /// de conexion) y divide el script en lotes por lineas GO.
        /// </summary>
        public static List<string> SplitBatches(string script)
        {
            var batches = new List<string>();
            var current = new StringBuilder();

            using (var reader = new StringReader(script))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string trimmed = line.Trim();

                    if (trimmed.StartsWith("USE ", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (string.Equals(trimmed, "GO", StringComparison.OrdinalIgnoreCase))
                    {
                        AddBatch(batches, current);
                        continue;
                    }

                    current.AppendLine(line);
                }
            }

            AddBatch(batches, current);
            return batches;
        }

        private static void AddBatch(List<string> batches, StringBuilder current)
        {
            string batch = current.ToString().Trim();

            if (batch.Length > 0)
            {
                batches.Add(batch);
            }

            current.Clear();
        }

        /// <summary>
        /// Crea la base de destino si no existe (01 asume que la base
        /// ya esta creada). Lanza la excepcion original ante cualquier fallo.
        /// </summary>
        public void EnsureDatabase()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            string database = builder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(database))
            {
                return;
            }

            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(
                    "DECLARE @sql NVARCHAR(4000) = N'CREATE DATABASE ' + QUOTENAME(@db); " +
                    "IF DB_ID(@db) IS NULL EXEC(@sql);",
                    connection))
                {
                    command.Parameters.AddWithValue("@db", database);
                    command.CommandTimeout = 300;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ejecuta un archivo .sql completo. Devuelve la cantidad de lotes
        /// ejecutados. Lanza la excepcion original ante cualquier fallo.
        /// </summary>
        public int RunFile(string path)
        {
            string script = File.ReadAllText(path, Encoding.UTF8);
            List<string> batches = SplitBatches(script);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (string batch in batches)
                {
                    using (var command = new SqlCommand(batch, connection))
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandTimeout = 300;
                        command.ExecuteNonQuery();
                    }
                }
            }

            return batches.Count;
        }
        #endregion
    }
}
