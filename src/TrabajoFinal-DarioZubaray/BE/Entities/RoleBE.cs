using BE.Composite;
using System.Collections.Generic;

namespace BE.Entities
{
    public class RoleBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PermissionBE> Permissions { get; set; }
        #endregion

        #region Constructor
        public RoleBE()
        {
            Permissions = new List<PermissionBE>();
        }
        #endregion

        #region Métodos
        public RoleBE(int id, string name, List<PermissionBE> permissions)
        {
            Id = id;
            Name = name;
            Permissions = permissions ?? new List<PermissionBE>();
        }

        public bool HasPermission(string permissionName)
        {
            return Permissions.Exists(p => p.Name == permissionName);
        }

        public RoleCompositeBE ToComposite(List<RoleBE> childRoles = null)
        {
            var composite = new RoleCompositeBE(Id, Name);

            foreach (var permission in Permissions)
            {
                composite.AddChild(new PermissionLeafBE(permission));
            }

            if (childRoles != null)
            {
                foreach (var childRole in childRoles)
                {
                    composite.AddChild(childRole.ToComposite());
                }
            }

            return composite;
        }

        public override string ToString()
        {
            return $"{Name} ({Permissions.Count} permissions)";
        }
        #endregion
    }
}
