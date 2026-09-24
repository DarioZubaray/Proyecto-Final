using System.Collections.Generic;

using BE.Entities;

namespace BE.Composite
{
    public interface IRoleComponentBE
    {
        string Name { get; }
        bool HasPermission(string permissionName);
        List<PermissionBE> GetAllPermissions();
    }
}
