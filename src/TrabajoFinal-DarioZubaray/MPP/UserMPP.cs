using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using BE;
using DAL;

namespace MPP
{
    public class UserMPP : IUserMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public UserMPP()
        {
            _access = new AccessDAL();
        }
        #endregion

        #region Métodos Públicos
        public UserBE GetByUserName(string userName)
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, role_id
                            FROM Users
                            WHERE user_name = @userName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userName", userName)
            };

            return FindOne(query, parameters);
        }

        public bool UpdateLastUpdate(int userId, DateTime lastUpdate)
        {
            string query = @"UPDATE Users
                            SET last_update = @lastUpdate
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@lastUpdate", lastUpdate),
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        public bool UpdateRetries(int userId, int retriesCount)
        {
            string query = @"UPDATE Users
                            SET retries_count = @retriesCount
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@retriesCount", retriesCount),
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        public bool Deactivate(int userId)
        {
            string query = @"UPDATE Users
                            SET is_active = 0, retries_count = 3
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        public bool Delete(UserBE user)
        {
            string query = @"UPDATE Users
                            SET is_active = 0
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", user.Id)
            };

            return _access.Save(query, parameters);
        }

        public bool Save(UserBE user)
        {
            if (user.Id == 0)
            {
                return Insert(user);
            }

            return Update(user);
        }

        public UserBE FindById(UserBE user)
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, role_id
                            FROM Users
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", user.Id)
            };

            return FindOne(query, parameters);
        }

        public List<UserBE> FindAll()
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, role_id
                            FROM Users";

            return FindMany(query);
        }

        public List<UserBE> FindByUserName(string userName)
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, role_id
                            FROM Users
                            WHERE user_name LIKE @userName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userName", "%" + userName + "%")
            };

            return FindMany(query, parameters);
        }
        public int CountByRoleId(int roleId)
        {
            string query = @"SELECT COUNT(*) FROM Users WHERE role_id = @roleId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };
            return _access.ReadScalar(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private UserBE MapUser(DataRow row)
        {
            return new UserBE
            {
                Id = Convert.ToInt32(row["id"]),
                UserName = row["user_name"].ToString(),
                PasswordHash = row["password_hash"].ToString(),
                IsActive = Convert.ToBoolean(row["is_active"]),
                RetriesCount = Convert.ToInt32(row["retries_count"]),
                LastUpdate = (DateTime)row["last_update"],
                CreatedAt = (DateTime)row["created_at"],
                Language = row["language"].ToString(),
                RoleId = row["role_id"] != DBNull.Value ? Convert.ToInt32(row["role_id"]) : 0
            };
        }

        private SqlParameter[] CreateUserParameters(UserBE user)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@userName", user.UserName),
                new SqlParameter("@passwordHash", user.PasswordHash),
                new SqlParameter("@isActive", user.IsActive),
                new SqlParameter("@retriesCount", user.RetriesCount),
                new SqlParameter("@lastUpdate", user.LastUpdate),
                new SqlParameter("@createdAt", user.CreatedAt),
                new SqlParameter("@language", user.Language ?? "es"),
                new SqlParameter("@roleId", user.RoleId)
            };
        }

        private bool Insert(UserBE user)
        {
            string query = @"INSERT INTO Users
                                (user_name, password_hash, is_active,
                                 retries_count, last_update, created_at, language, role_id)
                            VALUES
                                (@userName, @passwordHash, @isActive,
                                 @retriesCount, @lastUpdate, @createdAt, @language, @roleId);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = CreateUserParameters(user);
            var newId = _access.ReadScalar(query, parameters);
            return newId > 0;
        }

        private bool Update(UserBE user)
        {
            string query = @"UPDATE Users
                            SET user_name = @userName,
                                password_hash = @passwordHash,
                                is_active = @isActive,
                                retries_count = @retriesCount,
                                last_update = @lastUpdate,
                                created_at = @createdAt,
                                language = @language,
                                role_id = @roleId
                            WHERE id = @id";

            SqlParameter[] parameters = CreateUserParameters(user);
            var idParam = new SqlParameter("@id", user.Id);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = idParam;
            return _access.Save(query, allParameters);
        }

        public bool UpdateLanguage(int userId, string language)
        {
            string query = @"UPDATE Users
                            SET language = @language
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@language", language),
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        public bool UpdatePassword(int userId, string passwordHash)
        {
            string query = @"UPDATE Users
                            SET password_hash = @passwordHash
                            WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@passwordHash", passwordHash),
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        private UserBE FindOne(string query, SqlParameter[] parameters)
        {
            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            return MapUser(table.Rows[0]);
        }

        private List<UserBE> FindMany(string query)
        {
            List<UserBE> users = new List<UserBE>();
            DataTable table = _access.Read(query);

            foreach (DataRow row in table.Rows)
            {
                users.Add(MapUser(row));
            }

            return users;
        }

        private List<UserBE> FindMany(string query, SqlParameter[] parameters)
        {
            List<UserBE> users = new List<UserBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                users.Add(MapUser(row));
            }

            return users;
        }
        #endregion
    }
}
