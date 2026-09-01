using System;

using BE.DTOs;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using MPP;

namespace BLL.Services
{
    public class AuthBLL : IAuthBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;
        private const int MAX_RETRIES = 3;
        #endregion

        #region Constructor
        public AuthBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
        }
        #endregion

        #region Métodos Públicos
        public LoginResultBE Login(string userName, string password)
        {
            if (!AreCredentialsValid(userName, password))
            {
                return CreateLoginFailed(Resources.Auth_RequiredFields, ErrorCodesBLL.Auth.RequiredFields);
            }

            UserBE user = _userMPP.GetByUserName(userName);

            if (user == null)
            {
                return CreateLoginFailed(Resources.Auth_InvalidCredentials, ErrorCodesBLL.Auth.InvalidCredentials);
            }

            if (!user.IsActive)
            {
                return CreateLoginFailed(Resources.Auth_UserBlocked, ErrorCodesBLL.Auth.UserBlocked);
            }

            return AuthenticateUser(user, password);
        }

        public bool Logout(UserBE user)
        {
            return _userMPP.Save(user);
        }

        public bool TestConnection()
        {
            return _userMPP.TestConnection();
        }
        #endregion

        #region Métodos Privados
        private bool AreCredentialsValid(string userName, string password)
        {
            return !string.IsNullOrEmpty(userName)
                && !string.IsNullOrEmpty(password);
        }

        private LoginResultBE AuthenticateUser(UserBE user, string password)
        {
            bool isPasswordValid = EncryptionBLL.VerifyPassword(password, user.PasswordHash);

            if (isPasswordValid)
            {
                return LoginSuccessful(user);
            }

            return LoginFailed(user);
        }

        private LoginResultBE LoginSuccessful(UserBE user)
        {
            user.LastUpdate = DateTime.Now;
            _userMPP.UpdateLastUpdate(user.Id, user.LastUpdate);

            if (user.RetriesCount != 0)
            {
                user.RetriesCount = 0;
                _userMPP.UpdateRetries(user.Id, 0);
            }

            return new LoginResultBE
            {
                Success = true,
                Message = Resources.Auth_LoginSuccess,
                User = user
            };
        }

        private LoginResultBE LoginFailed(UserBE user)
        {
            user.RetriesCount++;

            if (user.RetriesCount >= MAX_RETRIES)
            {
                _userMPP.Deactivate(user.Id);
                return CreateLoginFailed(Resources.Auth_MaxRetriesExceeded, ErrorCodesBLL.Auth.MaxRetriesExceeded);
            }

            _userMPP.UpdateRetries(user.Id, user.RetriesCount);
            int retriesLeft = MAX_RETRIES - user.RetriesCount;
            return CreateLoginFailed(
                string.Format(Resources.Auth_RetriesLeft, retriesLeft),
                ErrorCodesBLL.Auth.RetryAttemptsLeft);
        }

        private LoginResultBE CreateLoginFailed(string message, string errorCode)
        {
            return new LoginResultBE { Success = false, Message = message, ErrorCode = errorCode };
        }
        #endregion
    }
}
