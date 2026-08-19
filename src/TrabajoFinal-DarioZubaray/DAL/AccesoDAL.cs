using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AccesoDAL
    {
        private SqlConnection conexion;
        SqlCommand sqlCommand;

        public AccesoDAL()
        {
            this.conexion = new SqlConnection();
            this.conexion.ConnectionString = ConfigurationManager.ConnectionStrings["cadenaConexion"].ToString();
        }

        #region Metodos Genericos
        public DataTable Leer(string consulta)
        {
            DataTable tabla = new DataTable();
            try
            {
                this.conexion.Open();
                SqlDataAdapter Da = new SqlDataAdapter(consulta, this.conexion);
                Da.Fill(tabla);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.conexion.Close();
            }
            return tabla;
        }

        public int LeerScalar(string consulta)
        {
            try
            {
                this.conexion.Open();
                sqlCommand = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    Connection = this.conexion,
                    CommandText = consulta
                };
                int respuesta = Convert.ToInt32(sqlCommand.ExecuteScalar());

                return respuesta;
            }
            catch
            {
                throw;
            }
            finally
            {
                this.conexion.Close();
            }
        }

        public bool Guardar(string consulta)
        {
            try
            {
                this.conexion.Open();
                sqlCommand = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    Connection = this.conexion,
                    CommandText = consulta
                };
                int respuesta = sqlCommand.ExecuteNonQuery();
                return respuesta > 0;
            }
            catch
            {
                throw;
            }
            finally
            {
                this.conexion.Close();
            }
        }
        #endregion

    }
}
