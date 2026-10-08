namespace BLL.Interfaces
{
    public interface ITwoFactorBLL
    {
        string ProviderName { get; }

        bool Verify(string userName, string code);
    }
}
