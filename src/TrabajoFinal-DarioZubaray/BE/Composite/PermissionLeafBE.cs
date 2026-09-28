using System.Collections.Generic;

using BE.Entities;

namespace BE.Composite
{
    public class PermissionLeafBE : RoleComponentBE
    {
        #region Propiedades
        private readonly PermissionBE _option;
        public override string Name => _option.Name;
        #endregion

        #region Constructor
        public PermissionLeafBE(PermissionBE option)
        {
            _option = option;
        }
        #endregion

        #region Métodos
        public override bool HasPermission(string permissionName)
        {
            return _option.Name == permissionName;
        }

        public override List<PermissionBE> GetAllPermissions()
        {
            return new List<PermissionBE> { _option };
        }

        public override string ToString()
        {
            return $"Permission: {_option.Label}";
        }
        #endregion
    }
}