using System.Collections.Generic;

namespace BE
{
    public class RoleBE
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<MenuOptionBE> Options { get; set; }

        public RoleBE()
        {
            Options = new List<MenuOptionBE>();
        }

        public RoleBE(int pId, string pName, List<MenuOptionBE> pOptions)
        {
            this.Id = pId;
            this.Name = pName;
            this.Options = pOptions ?? new List<MenuOptionBE>();
        }

        public bool HasOption(string optionName)
        {
            return Options.Exists(o => o.Name == optionName);
        }

        public override string ToString()
        {
            return $"{Name} ({Options.Count} opciones)";
        }
    }
}
