using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class PermissionBLL
    {
        #region Propiedades
        private readonly IRoleMPP _roleMPP;
        #endregion

        #region Constructor
        public PermissionBLL() : this(new RoleMPP())
        {
        }

        public PermissionBLL(IRoleMPP roleMPP)
        {
            _roleMPP = roleMPP;
        }
        #endregion

        #region Métodos Públicos
        public RoleCompositeBE BuildRoleTree(int roleId)
        {
            RoleBE role = _roleMPP.FindById(roleId);

            if (role == null)
            {
                return null;
            }

            List<int> childRoleIds = _roleMPP.GetChildRoleIds(roleId);
            List<RoleBE> childRoles = new List<RoleBE>();

            foreach (int childId in childRoleIds)
            {
                RoleBE childRole = _roleMPP.FindById(childId);
                if (childRole != null)
                {
                    childRoles.Add(childRole);
                }
            }

            return role.ToComposite(childRoles);
        }

        public bool HasPermission(RoleCompositeBE roleTree, string permissionName)
        {
            if (roleTree == null)
            {
                return false;
            }

            return roleTree.HasPermission(permissionName);
        }
        #endregion
    }
}
