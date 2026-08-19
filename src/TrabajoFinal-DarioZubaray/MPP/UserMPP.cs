using System;
using System.Collections.Generic;
using System.Data;

using BE;
using DAL;

namespace MPP
{
    public class UserMPP
    {
        private AccesoDAL _acceso;

        public UserMPP()
        {
            this._acceso = new AccesoDAL();
        }

        public bool Baja(UserBE objeto)
        {
            string consulta = string.Format(@"UPDATE Alumno SET activo = 0 WHERE legajo = {0}", objeto.Id);
            return this._acceso.Guardar(consulta);
        }

        public bool Guardar(UserBE objeto)
        {
            if (objeto.Id == 0)
            {
                string inserccionUsuario = string.Format(@"INSERT INTO User(user_name, password_hash, is_active, retries_count, last_update, created_at) 
                                                VALUES('{0}','{1}',{2},{3},'{4}','{5}');
                                                SELECT SCOPE_IDENTITY();",
                                                objeto.UserName, objeto.PasswordHash, objeto.IsActive, objeto.RetriesCount, objeto.LastUpdate, objeto.CreatedAt);

                var nuevoId = this._acceso.LeerScalar(inserccionUsuario);

                return nuevoId > 0;
            }
            else
            {
                string actualizacionUsuario = string.Format(@"UPDATE User(user_name, password_hash, is_active, retries_count, last_update, created_at) 
                                                VALUES('{0}','{1}',{2},{3},'{4}','{5}');
                                                SELECT SCOPE_IDENTITY();",
                                                objeto.UserName, objeto.PasswordHash, objeto.IsActive, objeto.RetriesCount, objeto.LastUpdate, objeto.CreatedAt);

                return this._acceso.Guardar(actualizacionUsuario);
            }
        }

        public UserBE ListarObjeto(UserBE objeto)
        {
            string Consulta = string.Format(@"SELECT a.legajo, a.nombre_apellido, a.documento, a.fecha_nacimiento, d.calle_numero, d.ciudad 
                                            FROM Alumno a
                                            INNER JOIN Direccion d ON a.legajo = d.id_legajo
                                            WHERE a.activo = 1 and a.legajo = {0}", objeto.Id);

            DataTable Tabla = this._acceso.Leer(Consulta);

            UserBE userDB = new UserBE();
            if (Tabla.Rows.Count > 0)
            {
                foreach (DataRow fila in Tabla.Rows)
                {
                    userDB.Id = Convert.ToInt32(fila[0]);
                    userDB.UserName = fila[1].ToString();
                    userDB.PasswordHash = fila[2].ToString();
                    userDB.IsActive = Convert.ToBoolean(fila[3]);
                    userDB.RetriesCount = Convert.ToInt32(fila[4]);
                    userDB.LastUpdate = (DateTime)fila[5];
                    userDB.CreatedAt = (DateTime)fila[6];
                }
            }
            return userDB;
        }

        public List<UserBE> ListarTodo()
        {
            List<UserBE> userList = new List<UserBE>();

            string Consulta = @"SELECT a.legajo, a.nombre_apellido, a.documento, a.fecha_nacimiento, 
                                        d.calle_numero, d.ciudad 
                                FROM Alumno a
                                INNER JOIN Direccion d ON a.legajo = d.id_legajo
                                WHERE a.activo = 1";

            DataTable Tabla = this._acceso.Leer(Consulta);

            if (Tabla.Rows.Count > 0)
            {
                foreach (DataRow fila in Tabla.Rows)
                {
                    UserBE userDB = new UserBE();

                    userDB.Id = Convert.ToInt32(fila[0]);
                    userDB.UserName = fila[1].ToString();
                    userDB.PasswordHash = fila[2].ToString();
                    userDB.IsActive = Convert.ToBoolean(fila[3]);
                    userDB.RetriesCount = Convert.ToInt32(fila[4]);
                    userDB.LastUpdate = (DateTime)fila[5];
                    userDB.CreatedAt = (DateTime)fila[6];
                    userList.Add(userDB);
                }
            }
            return userList;
        }
    }
}
