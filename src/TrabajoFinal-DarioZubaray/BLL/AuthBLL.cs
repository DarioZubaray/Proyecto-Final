using System;
using BE;
using MPP;

namespace BLL
{
    public class AuthBLL : IAuthBLL
    {
        #region Propiedades
        private readonly IUserMPP _userMPP;
        private const int MaxRetries = 3;
        #endregion

        #region Constructores
        public AuthBLL() : this(new MPP.UserMPP())
        {
        }

        public AuthBLL(IUserMPP userMPP)
        {
            _userMPP = userMPP;
        }
        #endregion

        #region Métodos
        public LoginResult Login(string userName, string password)
        {
            if (!CredencialesValidas(userName, password))
            {
                return CrearLoginFallido(Messages.Auth_RequiredFields);
            }

            UserBE user = _userMPP.ObtenerPorUserName(userName);

            if (user == null)
            {
                return CrearLoginFallido(Messages.Auth_InvalidCredentials);
            }

            if (!user.IsActive)
            {
                return CrearLoginFallido(Messages.Auth_UserBlocked);
            }

            return AutenticarUsuario(user, password);
        }

        public bool Logout(UserBE user)
        {
            return _userMPP.Guardar(user);
        }

        private bool CredencialesValidas(string userName, string password)
        {
            return !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password);
        }

        private LoginResult AutenticarUsuario(UserBE user, string password)
        {
            bool passwordValido = EncriptacionBLL.VerifyPassword(password, user.PasswordHash);

            if (passwordValido)
            {
                return LoginExitoso(user);
            }

            return LoginFallido(user);
        }

        private LoginResult LoginExitoso(UserBE user)
        {
            user.LastUpdate = DateTime.Now;
            _userMPP.ActualizarLastUpdate(user.Id, user.LastUpdate);

            if (user.RetriesCount != 0)
            {
                user.RetriesCount = 0;
                _userMPP.ActualizarRetries(user.Id, 0);
            }

            return new LoginResult
            {
                Success = true,
                Message = Messages.Auth_LoginSuccess,
                User = user
            };
        }

        private LoginResult LoginFallido(UserBE user)
        {
            user.RetriesCount++;

            if (user.RetriesCount >= MaxRetries)
            {
                _userMPP.Desactivar(user.Id);
                return CrearLoginFallido(Messages.Auth_MaxRetriesExceeded);
            }

            _userMPP.ActualizarRetries(user.Id, user.RetriesCount);
            int retriesLeft = MaxRetries - user.RetriesCount;
            return CrearLoginFallido(
                string.Format(Messages.Auth_RetriesLeft, retriesLeft));
        }

        private LoginResult CrearLoginFallido(string mensaje)
        {
            return new LoginResult { Success = false, Message = mensaje };
        }
        #endregion
    }
}
