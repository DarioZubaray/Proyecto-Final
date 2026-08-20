using System;

namespace BE
{
    public class MenuOptionBE
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }

        public MenuOptionBE() { }

        public MenuOptionBE(int pId, string pName, string pLabel, string pDescription, bool pIsGlobal)
        {
            this.Id = pId;
            this.Name = pName;
            this.Label = pLabel;
            this.Description = pDescription;
            this.IsGlobal = pIsGlobal;
        }

        public override string ToString()
        {
            return $"{Label}";
        }
    }
}
