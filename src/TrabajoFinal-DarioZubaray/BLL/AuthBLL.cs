using System;

using BE;
using BE.Properties;
using MPP;

namespace BLL
{
    public class AuthBLL : IAuthBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;
        private const int MAX_RETRIES = 3;
        #endregion

        #region Constructor
        public AuthBLL() : this(new MPP.UserMPP())
        {
        }

        public AuthBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
        }
        #endregion

        #region Métodos Públicos
        public LoginResultDTO Login(string userName, string password)
        {
            if (!AreCredentialsValid(userName, password))
            {
                return CreateLoginFailed(Resources.Auth_RequiredFields);
            }

            UserBE user = _userMPP.GetByUserName(userName);

            if (user == null)
            {
                return CreateLoginFailed(Resources.Auth_InvalidCredentials);
            }

            if (!user.IsActive)
            {
                return CreateLoginFailed(Resources.Auth_UserBlocked);
            }

            return AuthenticateUser(user, password);
        }

        public bool Logout(UserBE user)
        {
            return _userMPP.Save(user);
        }
        #endregion

        #region Métodos Privados
        private bool AreCredentialsValid(string userName, string password)
        {
            return !string.IsNullOrEmpty(userName)
                && !string.IsNullOrEmpty(password);
        }

        private LoginResultDTO AuthenticateUser(UserBE user, string password)
        {
            bool isPasswordValid = EncryptionBLL
                .VerifyPassword(password, user.PasswordHash);

            if (isPasswordValid)
            {
                return LoginSuccessful(user);
            }

            return LoginFailed(user);
        }

        private LoginResultDTO LoginSuccessful(UserBE user)
        {
            user.LastUpdate = DateTime.Now;
            _userMPP.UpdateLastUpdate(user.Id, user.LastUpdate);

            if (user.RetriesCount != 0)
            {
                user.RetriesCount = 0;
                _userMPP.UpdateRetries(user.Id, 0);
            }

            return new LoginResultDTO
            {
                Success = true,
                Message = Resources.Auth_LoginSuccess,
                User = user
            };
        }

        private LoginResultDTO LoginFailed(UserBE user)
        {
            user.RetriesCount++;

            if (user.RetriesCount >= MAX_RETRIES)
            {
                _userMPP.Deactivate(user.Id);
                return CreateLoginFailed(Resources.Auth_MaxRetriesExceeded);
            }

            _userMPP.UpdateRetries(user.Id, user.RetriesCount);
            int retriesLeft = MAX_RETRIES - user.RetriesCount;
            return CreateLoginFailed(
                string.Format(Resources.Auth_RetriesLeft, retriesLeft));
        }

        private LoginResultDTO CreateLoginFailed(string message)
        {
            return new LoginResultDTO { Success = false, Message = message };
        }
        #endregion
    }
}
