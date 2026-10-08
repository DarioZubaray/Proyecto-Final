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

        // Usuario admin: no se elimina, no se desactiva, no se renombra,
        // no cambia de rol.
        public const string ADMIN_USERNAME = "admin";
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
            // El admin nunca se elimina.
            if (user != null && IsAdminName(user.UserName))
            {
                return false;
            }

            return _userMPP.Delete(user);
        }

        public bool Save(UserBE user)
        {
            if (user == null)
            {
                return false;
            }

            // El admin no se renombra ni cambia de rol: se compara contra el
            // registro persistido, porque la GUI ya pudo haber pisado los valores.
            if (user.Id != 0 && IsAdminProtectedChange(user))
            {
                return false;
            }

            // El admin nunca se desactiva ni conserva reintentos:
            // se fuerzan siempre sus valores seguros.
            if (IsAdminName(user.UserName))
            {
                user.IsActive = true;
                user.RetriesCount = 0;
            }

            return _userMPP.Save(user);
        }

        public static bool IsAdminName(string userName)
        {
            return string.Equals(userName, ADMIN_USERNAME, System.StringComparison.OrdinalIgnoreCase);
        }

        private bool IsAdminProtectedChange(UserBE user)
        {
            UserBE persisted = _userMPP.FindById(new UserBE { Id = user.Id });

            if (persisted == null || !IsAdminName(persisted.UserName))
            {
                return false;
            }

            // Nombre cambiado, o rol cambiado (el rol define los permisos del admin).
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
            return _userMPP.UpdateLanguage(userId, language);
        }

        public bool UpdateTheme(int userId, string theme)
        {
            return _userMPP.UpdateTheme(userId, theme);
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
