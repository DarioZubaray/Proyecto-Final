using System.Collections.Generic;

using BE.Entities;

namespace BE.Composite
{
    public abstract class RoleComponentBE
    {
        #region Propiedades
        public abstract string Name { get; }
        #endregion

        #region Métodos
        public abstract bool HasPermission(string permissionName);
        public abstract List<PermissionBE> GetAllPermissions();
        #endregion
    }
}