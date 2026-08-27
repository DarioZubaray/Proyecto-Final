using System.Collections.Generic;

namespace BE
{
    public interface IRoleComponentBE
    {
        string Name { get; }
        bool HasPermission(string permissionName);
        List<PermissionBE> GetAllPermissions();
    }
}
