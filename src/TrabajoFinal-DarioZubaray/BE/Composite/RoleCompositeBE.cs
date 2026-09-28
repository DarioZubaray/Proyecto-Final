using System.Collections.Generic;

using BE.Entities;

namespace BE.Composite
{
    public class RoleCompositeBE : RoleComponentBE
    {
        #region Propiedades
        public int Id { get; set; }
        public override string Name { get; }
        private readonly List<RoleComponentBE> _children;
        #endregion

        #region Constructor
        public RoleCompositeBE()
        {
            _children = new List<RoleComponentBE>();
        }

        public RoleCompositeBE(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }
        #endregion

        #region Métodos
        public void AddChild(RoleComponentBE child)
        {
            _children.Add(child);
        }

        public void RemoveChild(RoleComponentBE child)
        {
            _children.Remove(child);
        }

        public List<RoleComponentBE> GetChildren()
        {
            return new List<RoleComponentBE>(_children);
        }

        public override bool HasPermission(string permissionName)
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

        public override List<PermissionBE> GetAllPermissions()
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