using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
        private UserBE MapearUsuario(DataRow fila)
        {
            return new UserBE
            {
                Id = Convert.ToInt32(fila["id"]),
                UserName = fila["user_name"].ToString(),
                PasswordHash = fila["password_hash"].ToString(),
                IsActive = Convert.ToBoolean(fila["is_active"]),
                RetriesCount = Convert.ToInt32(fila["retries_count"]),
                LastUpdate = (DateTime)fila["last_update"],
                CreatedAt = (DateTime)fila["created_at"]
            };
        }

        private SqlParameter[] CrearParametrosUsuario(UserBE user)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@userName", user.UserName),
                new SqlParameter("@passwordHash", user.PasswordHash),
                new SqlParameter("@isActive", user.IsActive),
                new SqlParameter("@retriesCount", user.RetriesCount),
                new SqlParameter("@lastUpdate", user.LastUpdate),
                new SqlParameter("@createdAt", user.CreatedAt)
            };
        }

        public UserBE ObtenerPorUserName(string userName)
        {
            string consulta = @"SELECT id, user_name, password_hash, is_active,
                                       retries_count, last_update, created_at
                                FROM Users
                                WHERE user_name = @userName";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@userName", userName)
            };

            return BuscarUno(consulta, parametros);
        }

        public bool ActualizarLastUpdate(int userId, DateTime lastUpdate)
        {
            string consulta = @"UPDATE Users
                                SET last_update = @lastUpdate
                                WHERE id = @id";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@lastUpdate", lastUpdate),
                new SqlParameter("@id", userId)
            };

            return this._acceso.Guardar(consulta, parametros);
        }

        public bool ActualizarRetries(int userId, int retriesCount)
        {
            string consulta = @"UPDATE Users
                                SET retries_count = @retriesCount
                                WHERE id = @id";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@retriesCount", retriesCount),
                new SqlParameter("@id", userId)
            };

            return this._acceso.Guardar(consulta, parametros);
        }

        public bool Desactivar(int userId)
        {
            string consulta = @"UPDATE Users
                                SET is_active = 0, retries_count = 3
                                WHERE id = @id";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@id", userId)
            };

            return this._acceso.Guardar(consulta, parametros);
        }

        public bool Baja(UserBE user)
        {
            string consulta = @"UPDATE Users
                                SET is_active = 0
                                WHERE id = @id";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@id", user.Id)
            };

            return this._acceso.Guardar(consulta, parametros);
        }

        public bool Guardar(UserBE user)
        {
            if (user.Id == 0)
            {
                return Insertar(user);
            }

            return Actualizar(user);
        }

        private bool Insertar(UserBE user)
        {
            string consulta = @"INSERT INTO Users
                                    (user_name, password_hash, is_active,
                                     retries_count, last_update, created_at)
                                VALUES
                                    (@userName, @passwordHash, @isActive,
                                     @retriesCount, @lastUpdate, @createdAt);
                                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parametros = CrearParametrosUsuario(user);
            var nuevoId = this._acceso.LeerScalar(consulta, parametros);
            return nuevoId > 0;
        }

        private bool Actualizar(UserBE user)
        {
            string consulta = @"UPDATE Users
                                SET user_name = @userName,
                                    password_hash = @passwordHash,
                                    is_active = @isActive,
                                    retries_count = @retriesCount,
                                    last_update = @lastUpdate,
                                    created_at = @createdAt
                                WHERE id = @id";

            SqlParameter[] parametros = CrearParametrosUsuario(user);
            return this._acceso.Guardar(consulta, parametros);
        }

        public UserBE ListarObjeto(UserBE objeto)
        {
            string consulta = @"SELECT id, user_name, password_hash, is_active,
                                       retries_count, last_update, created_at
                                FROM Users
                                WHERE id = @id";

            SqlParameter[] parametros = new SqlParameter[]
            {
                new SqlParameter("@id", objeto.Id)
            };

            return BuscarUno(consulta, parametros);
        }

        public List<UserBE> ListarTodo()
        {
            string consulta = @"SELECT id, user_name, password_hash, is_active,
                                       retries_count, last_update, created_at
                                FROM Users";

            return BuscarTodos(consulta);
        }

        private UserBE BuscarUno(string consulta, SqlParameter[] parametros)
        {
            DataTable tabla = this._acceso.Leer(consulta, parametros);

            if (tabla.Rows.Count == 0)
            {
                return null;
            }

            return MapearUsuario(tabla.Rows[0]);
        }

        private List<UserBE> BuscarTodos(string consulta)
        {
            List<UserBE> userList = new List<UserBE>();
            DataTable tabla = this._acceso.Leer(consulta);

            foreach (DataRow fila in tabla.Rows)
            {
                userList.Add(MapearUsuario(fila));
            }

            return userList;
        }
        #endregion
    }
}
