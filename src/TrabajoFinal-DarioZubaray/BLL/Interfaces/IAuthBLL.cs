using BE.DTOs;
using BE.Entities;

namespace BLL.Interfaces
{
    public interface IAuthBLL
    {
        LoginResultBE Login(string userName, string password);
        bool Logout(UserBE user);
        bool TestConnection();
    }
}
