using System.Collections.Generic;

namespace BE
{
    public class PermissionLeafBE : IRoleComponentBE
    {
        #region Propiedades
        private readonly PermissionBE _option;
        public string Name => _option.Name;
        #endregion

        #region Constructor
        public PermissionLeafBE(PermissionBE option)
        {
            _option = option;
        }
        #endregion

        #region Métodos
        public bool HasPermission(string permissionName)
        {
            return _option.Name == permissionName;
        }

        public List<PermissionBE> GetAllPermissions()
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
