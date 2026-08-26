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
        #endregion
    }
}
