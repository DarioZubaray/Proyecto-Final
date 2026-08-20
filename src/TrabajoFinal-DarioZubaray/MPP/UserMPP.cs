using System;
using System.Collections.Generic;
using System.Data;

using BE;
using DAL;

namespace MPP
{
    public class UserMPP
    {
        #region Propiedades
        private AccesoDAL _acceso;
        #endregion

        #region Constructores
        public UserMPP()
        {
            this._acceso = new AccesoDAL();
        }
        #endregion

        #region Métodos
        public UserBE ObtenerPorUserName(string userName)
        {
            string consulta = string.Format(@"SELECT id, user_name, password_hash, is_active, retries_count, last_update, created_at
                                              FROM Users WHERE user_name = '{0}'", userName);

            DataTable tabla = this._acceso.Leer(consulta);

            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = tabla.Rows[0];
            UserBE user = new UserBE();
            user.Id = Convert.ToInt32(fila["id"]);
            user.UserName = fila["user_name"].ToString();
            user.PasswordHash = fila["password_hash"].ToString();
            user.IsActive = Convert.ToBoolean(fila["is_active"]);
            user.RetriesCount = Convert.ToInt32(fila["retries_count"]);
            user.LastUpdate = (DateTime)fila["last_update"];
            user.CreatedAt = (DateTime)fila["created_at"];

            return user;
        }

        public bool ActualizarLastUpdate(int userId, DateTime lastUpdate)
        {
            string consulta = string.Format(@"UPDATE Users SET last_update = '{0}' WHERE id = {1}",
                                            lastUpdate.ToString("yyyy-MM-dd HH:mm:ss"), userId);
            return this._acceso.Guardar(consulta);
        }

        public bool ActualizarRetries(int userId, int retriesCount)
        {
            string consulta = string.Format(@"UPDATE Users SET retries_count = {0} WHERE id = {1}",
                                            retriesCount, userId);
            return this._acceso.Guardar(consulta);
        }

        public bool Desactivar(int userId)
        {
            string consulta = string.Format(@"UPDATE Users SET is_active = 0, retries_count = 3 WHERE id = {0}", userId);
            return this._acceso.Guardar(consulta);
        }

        public bool Baja(UserBE user)
        {
            string consulta = string.Format(@"UPDATE Users SET is_active = 0 WHERE id = {0}", user.Id);
            return this._acceso.Guardar(consulta);
        }

        public bool Guardar(UserBE user)
        {
            if (user.Id == 0)
            {
                string inserccionUsuario = string.Format(@"INSERT INTO Users(user_name, password_hash, is_active, retries_count, last_update, created_at) 
                                                VALUES('{0}','{1}',{2},{3},'{4}','{5}');
                                                SELECT SCOPE_IDENTITY();",
                                                user.UserName, user.PasswordHash, user.IsActive ? 1 : 0, user.RetriesCount, user.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"), user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

                var nuevoId = this._acceso.LeerScalar(inserccionUsuario);

                return nuevoId > 0;
            }
            else
            {
                string actualizacionUsuario = string.Format(@"UPDATE Users SET user_name='{0}', password_hash='{1}', is_active={2}, retries_count={3}, last_update='{4}', created_at='{5}' WHERE id={6}",
                    user.UserName, user.PasswordHash, user.IsActive ? 1 : 0, user.RetriesCount, user.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"), user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), user.Id);

                return this._acceso.Guardar(actualizacionUsuario);
            }
        }

        public UserBE ListarObjeto(UserBE objeto)
        {
            string consulta = string.Format(@"SELECT id, user_name, password_hash, is_active, retries_count, last_update, created_at
                                              FROM Users WHERE id = {0}", objeto.Id);

            DataTable tabla = this._acceso.Leer(consulta);

            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = tabla.Rows[0];
            UserBE userDB = new UserBE();
            userDB.Id = Convert.ToInt32(fila["id"]);
            userDB.UserName = fila["user_name"].ToString();
            userDB.PasswordHash = fila["password_hash"].ToString();
            userDB.IsActive = Convert.ToBoolean(fila["is_active"]);
            userDB.RetriesCount = Convert.ToInt32(fila["retries_count"]);
            userDB.LastUpdate = (DateTime)fila["last_update"];
            userDB.CreatedAt = (DateTime)fila["created_at"];

            return userDB;
        }

        public List<UserBE> ListarTodo()
        {
            List<UserBE> userList = new List<UserBE>();

            string consulta = @"SELECT id, user_name, password_hash, is_active, retries_count, last_update, created_at
                                FROM Users";

            DataTable tabla = this._acceso.Leer(consulta);

            if (tabla.Rows.Count > 0)
            {
                foreach (DataRow fila in tabla.Rows)
                {
                    UserBE userDB = new UserBE();
                    userDB.Id = Convert.ToInt32(fila["id"]);
                    userDB.UserName = fila["user_name"].ToString();
                    userDB.PasswordHash = fila["password_hash"].ToString();
                    userDB.IsActive = Convert.ToBoolean(fila["is_active"]);
                    userDB.RetriesCount = Convert.ToInt32(fila["retries_count"]);
                    userDB.LastUpdate = (DateTime)fila["last_update"];
                    userDB.CreatedAt = (DateTime)fila["created_at"];
                    userList.Add(userDB);
                }
            }
            return userList;
        }
        #endregion
    }
}
