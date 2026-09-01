using System.Collections.Generic;
using BE.Entities;

namespace MPP
{
    public interface IRoleMPP
    {
        RoleBE FindById(int roleId);
        List<RoleBE> FindAll();
        List<PermissionBE> GetPermissionsByRoleId(int roleId);
        List<int> GetChildRoleIds(int parentId);
        int Save(RoleBE role);
        void SavePermissions(int roleId, List<int> permissionIds);
        bool Delete(int roleId);
        List<PermissionBE> GetAllPermissions();
    }
}
