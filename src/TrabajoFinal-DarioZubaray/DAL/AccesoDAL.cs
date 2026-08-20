using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AccesoDAL
    {
        #region Propiedades
        private readonly string _connectionString;
        #endregion

        #region Constructores
        public AccesoDAL()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["cadenaConexion"].ToString();
        }
        #endregion

        #region Metodos Genericos
        public DataTable Leer(string consulta)
        {
            return Leer(consulta, null);
        }

        public DataTable Leer(string consulta, SqlParameter[] parametros)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(consulta, conexion))
            {
                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                using (var adapter = new SqlDataAdapter(command))
                {
                    var tabla = new DataTable();
                    conexion.Open();
                    adapter.Fill(tabla);
                    return tabla;
                }
            }
        }

        public int LeerScalar(string consulta)
        {
            return LeerScalar(consulta, null);
        }

        public int LeerScalar(string consulta, SqlParameter[] parametros)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(consulta, conexion))
            {
                command.CommandType = CommandType.Text;

                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                conexion.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool Guardar(string consulta)
        {
            return Guardar(consulta, null);
        }

        public bool Guardar(string consulta, SqlParameter[] parametros)
        {
            using (var conexion = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(consulta, conexion))
            {
                command.CommandType = CommandType.Text;

                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                conexion.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}
