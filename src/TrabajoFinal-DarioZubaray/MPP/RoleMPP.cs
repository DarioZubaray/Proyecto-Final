using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using BE;
using DAL;

namespace MPP
{
    public class RoleMPP : IRoleMPP
    {
        #region Propiedades
        private AccessDAL _access;
        #endregion

        #region Constructor
        public RoleMPP()
        {
            _access = new AccessDAL();
        }
        #endregion

        #region Métodos Públicos
        public RoleBE FindById(int roleId)
        {
            string query = @"SELECT id, name FROM Roles WHERE id = @id";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@id", roleId)
            };

            DataTable table = _access.Read(query, parameters);

            if (table.Rows.Count == 0)
            {
                return null;
            }

            DataRow row = table.Rows[0];
            var role = new RoleBE
            {
                Id = Convert.ToInt32(row["id"]),
                Name = row["name"].ToString(),
                Permissions = GetPermissionsByRoleId(roleId)
            };

            return role;
        }

        public List<RoleBE> FindAll()
        {
            string query = @"SELECT id, name FROM Roles";
            DataTable table = _access.Read(query);
            List<RoleBE> roles = new List<RoleBE>();

            foreach (DataRow row in table.Rows)
            {
                int roleId = Convert.ToInt32(row["id"]);
                roles.Add(new RoleBE
                {
                    Id = roleId,
                    Name = row["name"].ToString(),
                    Permissions = GetPermissionsByRoleId(roleId)
                });
            }

            return roles;
        }

        public List<PermissionBE> GetPermissionsByRoleId(int roleId)
        {
            string query = @"SELECT p.id, p.name, p.label, p.description, p.is_system
                            FROM Permissions p
                            INNER JOIN RolePermissions rp ON p.id = rp.permission_id
                            WHERE rp.role_id = @roleId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };

            DataTable table = _access.Read(query, parameters);
            List<PermissionBE> permissions = new List<PermissionBE>();

            foreach (DataRow row in table.Rows)
            {
                permissions.Add(new PermissionBE
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    Label = row["label"].ToString(),
                    Description = row["description"].ToString(),
                    IsSystem = row["is_system"] != DBNull.Value && Convert.ToBoolean(row["is_system"])
                });
            }

            return permissions;
        }

        public List<int> GetChildRoleIds(int parentId)
        {
            string query = @"SELECT child_role_id FROM RoleHierarchy WHERE parent_role_id = @parentId";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@parentId", parentId)
            };

            DataTable table = _access.Read(query, parameters);
            List<int> childIds = new List<int>();

            foreach (DataRow row in table.Rows)
            {
                childIds.Add(Convert.ToInt32(row["child_role_id"]));
            }

            return childIds;
        }

        public int Save(RoleBE role)
        {
            if (role.Id > 0)
            {
                string updateQuery = @"UPDATE Roles SET name = @name WHERE id = @id";
                SqlParameter[] updateParams = new SqlParameter[]
                {
                    new SqlParameter("@name", role.Name),
                    new SqlParameter("@id", role.Id)
                };
                _access.Save(updateQuery, updateParams);
                return role.Id;
            }
            else
            {
                string insertQuery = @"INSERT INTO Roles (name) VALUES (@name); SELECT SCOPE_IDENTITY();";
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@name", role.Name)
                };
                DataTable result = _access.Read(insertQuery, insertParams);
                return Convert.ToInt32(result.Rows[0][0]);
            }
        }

        public void SavePermissions(int roleId, List<int> permissionIds)
        {
            string deleteQuery = @"DELETE FROM RolePermissions WHERE role_id = @roleId";
            SqlParameter[] deleteParams = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };
            _access.Save(deleteQuery, deleteParams);

            foreach (int permId in permissionIds)
            {
                string insertQuery = @"INSERT INTO RolePermissions (role_id, permission_id) VALUES (@roleId, @permId)";
                SqlParameter[] insertParams = new SqlParameter[]
                {
                    new SqlParameter("@roleId", roleId),
                    new SqlParameter("@permId", permId)
                };
                _access.Save(insertQuery, insertParams);
            }
        }

        public bool Delete(int roleId)
        {
            string deletePermsQuery = @"DELETE FROM RolePermissions WHERE role_id = @roleId";
            SqlParameter[] deletePermsParams = new SqlParameter[]
            {
                new SqlParameter("@roleId", roleId)
            };
            _access.Save(deletePermsQuery, deletePermsParams);

            string deleteQuery = @"DELETE FROM Roles WHERE id = @id";
            SqlParameter[] deleteParams = new SqlParameter[]
            {
                new SqlParameter("@id", roleId)
            };
            return _access.Save(deleteQuery, deleteParams);
        }

        public List<PermissionBE> GetAllPermissions()
        {
            string query = @"SELECT id, name, label, description, is_system FROM Permissions";
            DataTable table = _access.Read(query);
            List<PermissionBE> permissions = new List<PermissionBE>();

            foreach (DataRow row in table.Rows)
            {
                permissions.Add(new PermissionBE
                {
                    Id = Convert.ToInt32(row["id"]),
                    Name = row["name"].ToString(),
                    Label = row["label"].ToString(),
                    Description = row["description"].ToString(),
                    IsSystem = row["is_system"] != DBNull.Value && Convert.ToBoolean(row["is_system"])
                });
            }

            return permissions;
        }
        #endregion
    }
}
