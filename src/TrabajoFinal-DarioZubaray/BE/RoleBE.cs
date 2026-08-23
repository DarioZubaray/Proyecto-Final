using System.Collections.Generic;

namespace BE
{
    public class RoleBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MenuOptionBE> Options { get; set; }
        #endregion

        #region Constructor
        public RoleBE()
        {
            Options = new List<MenuOptionBE>();
        }
        #endregion

        #region Métodos
        public RoleBE(int id, string name, List<MenuOptionBE> options)
        {
            Id = id;
            Name = name;
            Options = options ?? new List<MenuOptionBE>();
        }

        public bool HasOption(string optionName)
        {
            return Options.Exists(o => o.Name == optionName);
        }

        public override string ToString()
        {
            return $"{Name} ({Options.Count} options)";
        }
        #endregion
    }
}
