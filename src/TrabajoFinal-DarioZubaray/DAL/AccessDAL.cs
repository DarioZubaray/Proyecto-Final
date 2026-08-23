using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AccessDAL
    {
        #region Propiedades
        private readonly string _connectionString;
        #endregion

        #region Constructor
        public AccessDAL()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["cadenaConexion"].ToString();
        }
        #endregion

        #region Métodos
        public DataTable Read(string query)
        {
            return Read(query, null);
        }

        public DataTable Read(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var table = new DataTable();
                    connection.Open();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        public int ReadScalar(string query)
        {
            return ReadScalar(query, null);
        }

        public int ReadScalar(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool Save(string query)
        {
            return Save(query, null);
        }

        public bool Save(string query, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}
