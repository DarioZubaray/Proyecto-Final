using BE.DTOs;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAuthBLL
    {
        LoginResultBE Login(string userName, string password);
        bool ValidateTwoFactor(UserBE user, string code);
        bool Logout(UserBE user);
        bool TestConnection();
    }
}
