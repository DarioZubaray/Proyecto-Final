using System.Collections.Generic;

using BE.Entities;

namespace BLL.Interfaces
{
    public interface IUserBLL
    {
        bool Delete(UserBE user);
        bool Save(UserBE user);
        bool IsUsernameTaken(string userName);
        UserBE FindById(UserBE user);
        List<UserBE> FindAll();
        List<UserBE> FindAllActives();
        List<UserBE> FindByUserName(string userName);
        bool UpdateLanguage(int userId, string language);
        bool UpdateTheme(int userId, string theme);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        int CountByRoleId(int roleId);
        bool HasRegisteredUsers();
    }
}
