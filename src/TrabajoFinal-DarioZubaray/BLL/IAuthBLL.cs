using BE;

namespace BLL
{
    public interface IAuthBLL
    {
        LoginResult Login(string userName, string password);
        bool Logout(UserBE user);
    }
}
