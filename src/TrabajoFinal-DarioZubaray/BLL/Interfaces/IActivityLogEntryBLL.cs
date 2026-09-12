namespace BLL.Interfaces
{
    public interface IActivityLogEntryBLL
    {
        int UserId { get; }
        string Action { get; }
        string FormName { get; }
        string Description { get; }
        bool Execute();
    }
}
