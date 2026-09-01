using BE.Entities;
using System.Collections.Generic;

namespace BE.Composite
{
    public interface IRoleComponentBE
    {
        string Name { get; }
        bool HasPermission(string permissionName);
        List<PermissionBE> GetAllPermissions();
    }
}
