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

        public override string ToString()
        {
            return Label;
        }
    }
}
