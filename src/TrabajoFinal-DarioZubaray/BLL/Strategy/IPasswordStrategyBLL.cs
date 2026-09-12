namespace BLL.Strategy
{
    public interface IPasswordStrategyBLL
    {
        bool Matches(string storedHash);

        string Hash(string password);

        bool Verify(string plain, string stored);
    }
}
