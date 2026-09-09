using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using DAL;
using BE.Entities;

namespace MPP
{
    public class ActivityMPP : IActivityMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public ActivityMPP() : this(null)
        {
        }

        public ActivityMPP(string connectionString)
        {
            _access = new AccessDAL(connectionString);
        }
        #endregion

        #region Métodos Públicos
        public bool Save(ActivityLogBE log)
        {
            string query = @"INSERT INTO ActivityLogs (user_id, action, form_name, description, created_at)
                            VALUES (@userId, @action, @formName, @description, @createdAt)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", log.UserId),
                new SqlParameter("@action", log.Action ?? (object)DBNull.Value),
                new SqlParameter("@formName", log.FormName ?? (object)DBNull.Value),
                new SqlParameter("@description", log.Description ?? (object)DBNull.Value),
                new SqlParameter("@createdAt", log.CreatedAt)
            };

            return _access.Save(query, parameters);
        }

        public List<ActivityLogBE> GetByUserPaginated(int userId, int page, int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            int offset = (page - 1) * pageSize;

            string query = @"SELECT id, user_id, action, form_name, description, created_at
                            FROM ActivityLogs
                            WHERE user_id = @userId
                            ORDER BY created_at DESC
                            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId),
                new SqlParameter("@offset", offset),
                new SqlParameter("@pageSize", pageSize)
            };

            List<ActivityLogBE> logs = new List<ActivityLogBE>();
            DataTable table = _access.Read(query, parameters);

            foreach (DataRow row in table.Rows)
            {
                logs.Add(MapLog(row));
            }

            return logs;
        }

        public int CountByUser(int userId)
        {
            string query = @"SELECT COUNT(*) FROM ActivityLogs WHERE user_id = @userId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@userId", userId)
            };

            return _access.ReadScalar(query, parameters);
        }
        #endregion

        #region Métodos Privados
        private ActivityLogBE MapLog(DataRow row)
        {
            return new ActivityLogBE
            {
                Id = Convert.ToInt32(row["id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                Action = row["action"] == DBNull.Value ? null : row["action"].ToString(),
                FormName = row["form_name"] == DBNull.Value ? null : row["form_name"].ToString(),
                Description = row["description"] == DBNull.Value ? null : row["description"].ToString(),
                CreatedAt = (DateTime)row["created_at"]
            };
        }
        #endregion
    }
}
