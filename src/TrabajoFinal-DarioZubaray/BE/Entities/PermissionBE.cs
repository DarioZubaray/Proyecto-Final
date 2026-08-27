namespace BE
{
    public class PermissionBE
    {
        #region Propiedades
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        #endregion

        #region Constructor
        public PermissionBE() { }

        public PermissionBE(int id, string name, string label, string description, bool isSystem = false)
        {
            Id = id;
            Name = name;
            Label = label;
            Description = description;
            IsSystem = isSystem;
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
