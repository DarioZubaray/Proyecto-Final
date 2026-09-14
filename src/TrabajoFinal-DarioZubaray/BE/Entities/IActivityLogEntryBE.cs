namespace BE.Entities
{
    public interface IActivityLogEntryBE
    {
        int UserId { get; }
        string Action { get; }
        string FormName { get; }
        string Description { get; }
        bool Execute();
    }
}
