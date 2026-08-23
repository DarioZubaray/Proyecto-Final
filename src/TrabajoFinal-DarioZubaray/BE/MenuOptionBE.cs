namespace BE
{
    public class MenuOptionBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsGlobal { get; set; }
        #endregion

        #region Constructor
        public MenuOptionBE() { }

        public MenuOptionBE(
            int id, string name, string label,
            string description, bool isGlobal)
        {
            Id = id;
            Name = name;
            Label = label;
            Description = description;
            IsGlobal = isGlobal;
        }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return Label;
        }
        #endregion
    }
}
