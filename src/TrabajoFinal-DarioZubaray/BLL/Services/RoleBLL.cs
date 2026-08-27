using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class RoleBLL : IRoleBLL
    {
        #region Propiedades
        private readonly IRoleMPP _roleMPP;
        #endregion

        #region Constructor
        public RoleBLL() : this(new MPP.RoleMPP())
        {
        }

        public RoleBLL(IRoleMPP roleMPP)
        {
            _roleMPP = roleMPP;
        }
        #endregion

        #region Métodos
        public RoleBE FindById(int roleId)
        {
            return _roleMPP.FindById(roleId);
        }

        public List<RoleBE> FindAll()
        {
            return _roleMPP.FindAll();
        }

        public List<PermissionBE> GetPermissionsByRoleId(int roleId)
        {
            return _roleMPP.GetPermissionsByRoleId(roleId);
        }

        public List<int> GetChildRoleIds(int parentId)
        {
            return _roleMPP.GetChildRoleIds(parentId);
        }

        public int Save(RoleBE role)
        {
            return _roleMPP.Save(role);
        }

        public void SavePermissions(int roleId, List<int> permissionIds)
        {
            List<PermissionBE> currentPerms = _roleMPP.GetPermissionsByRoleId(roleId);
            List<int> systemPermIds = currentPerms
                .FindAll(p => p.IsSystem)
                .ConvertAll(p => p.Id);

            foreach (int sysId in systemPermIds)
            {
                if (!permissionIds.Contains(sysId))
                {
                    permissionIds.Add(sysId);
                }
            }

            _roleMPP.SavePermissions(roleId, permissionIds);
        }

        public bool Delete(int roleId)
        {
            return _roleMPP.Delete(roleId);
        }

        public List<PermissionBE> GetAllPermissions()
        {
            return _roleMPP.GetAllPermissions();
        }
        #endregion
    }
}
