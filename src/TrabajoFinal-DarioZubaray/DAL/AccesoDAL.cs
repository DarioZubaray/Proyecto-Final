using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class AccesoDAL
    {
        #region Propiedades
        private SqlConnection conexion;
        SqlCommand sqlCommand;
        #endregion

        #region Constructores
        public AccesoDAL()
        {
            this.conexion = new SqlConnection();
            this.conexion.ConnectionString = ConfigurationManager.ConnectionStrings["cadenaConexion"].ToString();
        }
        #endregion

        #region Metodos Genericos
        public DataTable Leer(string consulta)
        {
            return Leer(consulta, null);
        }

        public DataTable Leer(string consulta, SqlParameter[] parametros)
        {
            DataTable tabla = new DataTable();
            try
            {
                this.conexion.Open();
                SqlDataAdapter Da = new SqlDataAdapter(consulta, this.conexion);
                if (parametros != null)
                {
                    Da.SelectCommand.Parameters.AddRange(parametros);
                }
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
            return LeerScalar(consulta, null);
        }

        public int LeerScalar(string consulta, SqlParameter[] parametros)
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
                if (parametros != null)
                {
                    sqlCommand.Parameters.AddRange(parametros);
                }
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
            return Guardar(consulta, null);
        }

        public bool Guardar(string consulta, SqlParameter[] parametros)
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
                if (parametros != null)
                {
                    sqlCommand.Parameters.AddRange(parametros);
                }
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
