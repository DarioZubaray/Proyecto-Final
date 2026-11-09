namespace BE.Entities
{
    public interface IActivityLogEntryBE
    {
        #region Propiedades
        int UserId { get; }
        string Action { get; }
        string FormName { get; }
        string Description { get; }
        #endregion

        #region Métodos
        bool Execute();
        #endregion
    }
}
