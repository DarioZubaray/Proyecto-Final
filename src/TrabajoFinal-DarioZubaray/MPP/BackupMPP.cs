using System;
using System.Configuration;
using Microsoft.Data.SqlClient;

using MPP.Interfaces;

namespace MPP
{
    public class BackupMPP : IBackupMPP
    {
        #region Propiedades
        private readonly string _connectionString;
        private readonly string _databaseName;
        #endregion

        #region Constructor
        public BackupMPP() : this(null)
        {
        }

        public BackupMPP(string connectionString)
        {
            if (connectionString == null)
            {
                connectionString = ConfigurationManager
                    .ConnectionStrings["cadenaConexion"].ConnectionString;
            }

            _connectionString = connectionString;
            _databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
        }
        #endregion

        #region Métodos Públicos
        public bool CreateBackup(string filePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(
                    $"BACKUP DATABASE [{_databaseName}] TO DISK = @filePath WITH INIT, FORMAT", connection))
                {
                    command.Parameters.AddWithValue("@filePath", filePath);
                    command.CommandTimeout = 300;
                    command.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public bool RestoreBackup(string filePath)
        {
            using (var masterConnection = new SqlConnection(GetMasterConnectionString()))
            {
                masterConnection.Open();

                using (var command = new SqlCommand(
                    $"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", masterConnection))
                {
                    command.ExecuteNonQuery();
                }

                try
                {
                    using (var command = new SqlCommand(
                        $"RESTORE DATABASE [{_databaseName}] FROM DISK = @filePath WITH REPLACE", masterConnection))
                    {
                        command.Parameters.AddWithValue("@filePath", filePath);
                        command.CommandTimeout = 300;
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    using (var command = new SqlCommand(
                        $"ALTER DATABASE [{_databaseName}] SET MULTI_USER", masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
        }
        #endregion

        #region Métodos Privados
        private string GetMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            return builder.ConnectionString;
        }
        #endregion
    }
}
