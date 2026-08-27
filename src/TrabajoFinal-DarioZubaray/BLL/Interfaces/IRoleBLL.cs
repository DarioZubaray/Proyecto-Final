using System.Collections.Generic;

using BE;

namespace BLL
{
    public interface IRoleBLL
    {
        RoleBE FindById(int roleId);
        List<RoleBE> FindAll();
        List<PermissionBE> GetPermissionsByRoleId(int roleId);
        List<int> GetChildRoleIds(int parentId);
    }
}
