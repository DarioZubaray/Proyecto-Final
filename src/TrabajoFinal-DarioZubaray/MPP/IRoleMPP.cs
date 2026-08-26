using System.Collections.Generic;

using BE;

namespace MPP
{
    public interface IRoleMPP
    {
        RoleBE FindById(int roleId);
        List<RoleBE> FindAll();
        List<PermissionBE> GetPermissionsByRoleId(int roleId);
        List<int> GetChildRoleIds(int parentId);
    }
}
