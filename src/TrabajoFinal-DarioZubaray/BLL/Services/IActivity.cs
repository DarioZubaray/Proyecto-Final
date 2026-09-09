namespace BLL.Services
{
    public interface IActivity
    {
        int UserId { get; }
        string Action { get; }
        string FormName { get; }
        string Description { get; }
        bool Execute();
    }
}
