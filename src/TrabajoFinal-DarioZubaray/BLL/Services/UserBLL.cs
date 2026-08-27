using System.Collections.Generic;

using BE;
using MPP;

namespace BLL
{
    public class UserBLL : IUserBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;
        #endregion

        #region Constructor
        public UserBLL() : this(new MPP.UserMPP())
        {
        }

        public UserBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
        }
        #endregion

        #region Métodos
        public bool Delete(UserBE user)
        {
            return _userMPP.Delete(user);
        }

        public bool Save(UserBE user)
        {
            return _userMPP.Save(user);
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
            return _userMPP.UpdateLanguage(userId, language);
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
            return _userMPP.UpdatePassword(userId, newHash);
        }

        public int CountByRoleId(int roleId)
        {
            return _userMPP.CountByRoleId(roleId);
        }
        #endregion
    }
}
