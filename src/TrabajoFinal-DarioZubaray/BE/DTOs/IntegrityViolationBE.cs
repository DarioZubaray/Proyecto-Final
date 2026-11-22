namespace BE.DTOs
{
    public class IntegrityViolationBE
    {
        #region Propiedades
        public string TableName { get; set; }
        public int RowId { get; set; }
        public string Description { get; set; }
        public string StoredHash { get; set; }
        public string ComputedHash { get; set; }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return $"{TableName}#{RowId}: {Description}";
        }
        #endregion
    }
}
