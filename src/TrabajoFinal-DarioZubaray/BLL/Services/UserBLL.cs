using System.Collections.Generic;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class UserBLL : IUserBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;

        public const string ADMIN_USERNAME = "admin";
        public const int SYSADMIN_ID = 1;
        #endregion

        #region Constructor
        public UserBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
        }
        #endregion

        #region Métodos
        public bool Delete(UserBE user)
        {
            if (user != null && IsAdminName(user.UserName))
            {
                return false;
            }

            return _userMPP.Delete(user) && DVHHelperBLL.Regenerate(_userMPP, user.Id);
        }

        public bool Save(UserBE user)
        {
            if (user == null)
            {
                return false;
            }

            if (user.Id != 0 && IsAdminProtectedChange(user))
            {
                return false;
            }

            if (IsAdminName(user.UserName))
            {
                user.IsActive = true;
                user.RetriesCount = 0;
            }

            return _userMPP.Save(user) && DVHHelperBLL.Regenerate(_userMPP, user);
        }

        public static bool IsAdminName(string userName)
        {
            return string.Equals(userName, ADMIN_USERNAME, System.StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSysAdmin(UserBE user)
        {
            return user != null && user.Id == SYSADMIN_ID && IsAdminName(user.UserName);
        }

        private bool IsAdminProtectedChange(UserBE user)
        {
            UserBE persisted = _userMPP.FindById(new UserBE { Id = user.Id });

            if (persisted == null || !IsAdminName(persisted.UserName))
            {
                return false;
            }

            return !IsAdminName(user.UserName) || persisted.RoleId != user.RoleId;
        }

        public UserBE FindById(UserBE user)
        {
            return _userMPP.FindById(user);
        }

        public List<UserBE> FindAll()
        {
            return _userMPP.FindAll();
        }

        public List<UserBE> FindByUserName(string userName)
        {
            return _userMPP.FindByUserName(userName);
        }

        public bool UpdateLanguage(int userId, string language)
        {
            return _userMPP.UpdateLanguage(userId, language)
                && DVHHelperBLL.Regenerate(_userMPP, userId);
        }

        public bool UpdateTheme(int userId, string theme)
        {
            return _userMPP.UpdateTheme(userId, theme)
                && DVHHelperBLL.Regenerate(_userMPP, userId);
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            UserBE user = _userMPP.FindById(new UserBE { Id = userId });

            if (user == null)
            {
                return false;
            }

            bool isValid = EncryptionBLL.VerifyPassword(currentPassword, user.PasswordHash);

            if (!isValid)
            {
                return false;
            }

            string newHash = EncryptionBLL.HashPassword(newPassword);
            return _userMPP.UpdatePassword(userId, newHash)
                && DVHHelperBLL.Regenerate(_userMPP, userId);
        }

        public int CountByRoleId(int roleId)
        {
            return _userMPP.CountByRoleId(roleId);
        }
        #endregion
    }
}
