using System.Collections.Generic;

namespace BE
{
    public class RoleComposite : IRoleComponent
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        private readonly List<IRoleComponent> _children;
        #endregion

        #region Constructor
        public RoleComposite()
        {
            _children = new List<IRoleComponent>();
        }

        public RoleComposite(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }
        #endregion

        #region Métodos
        public void AddChild(IRoleComponent child)
        {
            _children.Add(child);
        }

        public void RemoveChild(IRoleComponent child)
        {
            _children.Remove(child);
        }

        public List<IRoleComponent> GetChildren()
        {
            return new List<IRoleComponent>(_children);
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

        public List<MenuOptionBE> GetAllPermissions()
        {
            var permissions = new List<MenuOptionBE>();
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
