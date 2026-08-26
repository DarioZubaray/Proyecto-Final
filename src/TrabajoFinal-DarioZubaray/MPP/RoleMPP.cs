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
            string query = @"SELECT p.id, p.name, p.label, p.description
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
                    Description = row["description"].ToString()
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
        #endregion
    }
}
