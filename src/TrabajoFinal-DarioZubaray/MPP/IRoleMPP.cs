using System.Collections.Generic;

using BE;

namespace MPP
{
    public interface IRoleMPP
    {
        RoleBE FindById(int roleId);
        List<RoleBE> FindAll();
        List<MenuOptionBE> GetMenuOptionsByRoleId(int roleId);
        List<int> GetChildRoleIds(int parentId);
    }
}
