using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

using DAL;
using BE.Entities;
using MPP.Interfaces;

namespace MPP
{
    public class UserMPP : IUserMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public UserMPP() : this(null)
        {
        }

        public UserMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public UserBE GetByUserName(string userName)
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, theme, role_id, dvh
                            FROM users
                            WHERE user_name = @userName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userName", userName)
            };

            return FindOne(query, parameters);
        }

        public bool UpdateLastUpdate(int userId, DateTime lastUpdate)
        {
            return Mutate(userId, user => user.LastUpdate = lastUpdate);
        }

        public bool UpdateRetries(int userId, int retriesCount)
        {
            return Mutate(userId, user => user.RetriesCount = retriesCount);
        }

        public bool Deactivate(int userId)
        {
            return Mutate(userId, user =>
            {
                user.IsActive = false;
                user.RetriesCount = 3;
            });
        }

        public bool Delete(UserBE user)
        {
            return Mutate(user.Id, current => current.IsActive = false);
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
                                   language, theme, role_id, dvh
                            FROM users
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
                                   language, theme, role_id, dvh
                            FROM users";

            return FindMany(query);
        }

        public List<UserBE> FindAllActives()
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, theme, role_id, dvh
                            FROM users
                            WHERE is_active = 1";

            return FindMany(query);
        }

        public List<UserBE> FindByUserName(string userName)
        {
            string query = @"SELECT id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, theme, role_id, dvh
                            FROM users
                            WHERE user_name LIKE @userName";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userName", "%" + userName + "%")
            };

            return FindMany(query, parameters);
        }
        public int CountByRoleId(int roleId)
        {
            string query = @"SELECT COUNT(*) FROM users WHERE role_id = @roleId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };
            return _access.ReadScalar(query, parameters);
        }

        public bool TestConnection()
        {
            return _access.TestConnection();
        }
        #endregion

        #region Métodos Privados
        private bool Mutate(int userId, Action<UserBE> mutate)
        {
            UserBE current = FindById(new UserBE { Id = userId });

            if (current == null)
            {
                return false;
            }

            mutate(current);
            return Update(current);
        }

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
                Theme = row["theme"] != DBNull.Value ? row["theme"].ToString() : "System",
                RoleId = row["role_id"] != DBNull.Value ? Convert.ToInt32(row["role_id"]) : 0,
                Dvh = row["dvh"] != DBNull.Value ? row["dvh"].ToString() : null
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
                new SqlParameter("@theme", user.Theme ?? "System"),
                new SqlParameter("@roleId", user.RoleId)
            };
        }

        private const string PersistedColumns = @"id, user_name, password_hash, is_active,
                                   retries_count, last_update, created_at,
                                   language, theme, role_id, dvh";

        private bool Insert(UserBE user)
        {
            string query = @"INSERT INTO users
                                (user_name, password_hash, is_active,
                                 retries_count, last_update, created_at, language, theme, role_id)
                            VALUES
                                (@userName, @passwordHash, @isActive,
                                 @retriesCount, @lastUpdate, @createdAt, @language, @theme, @roleId);
                            SELECT " + PersistedColumns + @"
                            FROM users
                            WHERE id = SCOPE_IDENTITY();";

            return WriteAndReload(query, CreateUserParameters(user), user);
        }

        private bool Update(UserBE user)
        {
            string query = @"UPDATE users
                            SET user_name = @userName,
                                password_hash = @passwordHash,
                                is_active = @isActive,
                                retries_count = @retriesCount,
                                last_update = @lastUpdate,
                                created_at = @createdAt,
                                language = @language,
                                theme = @theme,
                                role_id = @roleId
                            WHERE id = @id;
                            SELECT " + PersistedColumns + @"
                            FROM users
                            WHERE id = @id";

            SqlParameter[] parameters = CreateUserParameters(user);
            var allParameters = new SqlParameter[parameters.Length + 1];
            parameters.CopyTo(allParameters, 0);
            allParameters[parameters.Length] = new SqlParameter("@id", user.Id);

            return WriteAndReload(query, allParameters, user);
        }

        private bool WriteAndReload(string query, SqlParameter[] parameters, UserBE user)
        {
            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return false;
            }

            UserBE persisted = MapUser(table.Rows[0]);
            user.Id = persisted.Id;
            user.LastUpdate = persisted.LastUpdate;
            user.CreatedAt = persisted.CreatedAt;
            user.Dvh = persisted.Dvh;

            return true;
        }

        public bool UpdateDvh(int userId, string dvh)
        {
            string query = @"UPDATE users
                             SET dvh = @dvh
                             WHERE id = @id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@dvh", dvh),
                new SqlParameter("@id", userId)
            };

            return _access.Save(query, parameters);
        }

        public bool UpdateLanguage(int userId, string language)
        {
            return Mutate(userId, user => user.Language = language);
        }

        public bool UpdateTheme(int userId, string theme)
        {
            return Mutate(userId, user => user.Theme = theme);
        }

        public bool UpdatePassword(int userId, string passwordHash)
        {
            return Mutate(userId, user => user.PasswordHash = passwordHash);
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
