using BE;

namespace BLL
{
    public interface IAuthBLL
    {
        LoginResultBE Login(string userName, string password);
        bool Logout(UserBE user);
        bool TestConnection();
    }
}
