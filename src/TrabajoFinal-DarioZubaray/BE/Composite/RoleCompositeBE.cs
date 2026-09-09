using BE.Entities;
using System.Collections.Generic;

namespace BE.Composite
{
    public class RoleCompositeBE : IRoleComponentBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        private readonly List<IRoleComponentBE> _children;
        #endregion

        #region Constructor
        public RoleCompositeBE()
        {
            _children = new List<IRoleComponentBE>();
        }

        public RoleCompositeBE(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }
        #endregion

        #region Métodos
        public void AddChild(IRoleComponentBE child)
        {
            _children.Add(child);
        }

        public void RemoveChild(IRoleComponentBE child)
        {
            _children.Remove(child);
        }

        public List<IRoleComponentBE> GetChildren()
        {
            return new List<IRoleComponentBE>(_children);
        }

        public bool HasPermission(string permissionName)
        {
            foreach (var child in _children)
            {
                if (child.HasPermission(permissionName))
                {
                    return true;
                }
            }
            return false;
        }

        public List<PermissionBE> GetAllPermissions()
        {
            var permissions = new List<PermissionBE>();
            foreach (var child in _children)
            {
                permissions.AddRange(child.GetAllPermissions());
            }
            return permissions;
        }

        public override string ToString()
        {
            return $"Role: {Name} ({_children.Count} children)";
        }
        #endregion
    }
}
