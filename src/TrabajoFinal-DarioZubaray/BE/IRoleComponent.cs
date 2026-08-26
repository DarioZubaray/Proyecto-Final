using System.Collections.Generic;

namespace BE
{
    public interface IRoleComponent
    {
        string Name { get; }
        bool HasPermission(string permissionName);
        List<PermissionBE> GetAllPermissions();
    }
}
