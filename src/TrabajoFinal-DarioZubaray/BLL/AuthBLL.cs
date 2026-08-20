using System;
using BE;
using MPP;

namespace BLL
{
    public class AuthBLL
    {
        #region Propiedades
        private UserMPP _userMPP;
        private const int MaxRetries = 3;
        #endregion

        #region Constructores
        public AuthBLL()
        {
            _userMPP = new UserMPP();
        }
        #endregion

        #region Métodos
        public LoginResult Login(string userName, string password)
        {
            if (!CredencialesValidas(userName, password))
            {
                return CrearLoginFallido("Usuario y contraseña son obligatorios.");
            }

            UserBE user = _userMPP.ObtenerPorUserName(userName);

            if (user == null)
            {
                return CrearLoginFallido("Usuario o contraseña incorrectos.");
            }

            if (!user.IsActive)
            {
                return CrearLoginFallido("Usuario bloqueado. Contacte al administrador.");
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
            string hashedPassword = EncriptacionBLL.HashPassword(password);

            if (user.PasswordHash == hashedPassword)
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
                Message = "Login exitoso.",
                User = user
            };
        }

        private LoginResult LoginFallido(UserBE user)
        {
            user.RetriesCount++;

            if (user.RetriesCount >= MaxRetries)
            {
                _userMPP.Desactivar(user.Id);
                return CrearLoginFallido(
                    "Usuario bloqueado por exceso de intentos fallidos. Contacte al administrador.");
            }

            _userMPP.ActualizarRetries(user.Id, user.RetriesCount);
            int retriesLeft = MaxRetries - user.RetriesCount;
            return CrearLoginFallido(
                $"Usuario o contraseña incorrectos. Intentos restantes: {retriesLeft}");
        }

        private LoginResult CrearLoginFallido(string mensaje)
        {
            return new LoginResult { Success = false, Message = mensaje };
        }
        #endregion
    }
}
