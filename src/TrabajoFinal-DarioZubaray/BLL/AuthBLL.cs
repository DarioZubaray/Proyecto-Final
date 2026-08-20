using System;
using BE;
using MPP;

namespace BLL
{
    public class AuthBLL
    {
        #region Propiedades
        private UserMPP mapeador;
        private const int MaxRetries = 3;
        #endregion

        #region Constructores
        public AuthBLL()
        {
            mapeador = new UserMPP();
        }
        #endregion

        #region Métodos
        public LoginResult Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return new LoginResult { Success = false, Message = "Usuario y contraseña son obligatorios." };
            }

            UserBE user = mapeador.ObtenerPorUserName(userName);

            if (user == null)
            {
                return new LoginResult { Success = false, Message = "Usuario o contraseña incorrectos." };
            }

            if (!user.IsActive)
            {
                return new LoginResult { Success = false, Message = "Usuario bloqueado. Contacte al administrador." };
            }

            string hashedPassword = EncriptacionBLL.HashPassword(password);

            if (user.PasswordHash == hashedPassword)
            {
                user.LastUpdate = DateTime.Now;
                mapeador.ActualizarLastUpdate(user.Id, user.LastUpdate);

                if (user.RetriesCount != 0)
                {
                    user.RetriesCount = 0;
                    mapeador.ActualizarRetries(user.Id, 0);
                }

                return new LoginResult { Success = true, Message = "Login exitoso.", User = user };
            }
            else
            {
                user.RetriesCount++;

                if (user.RetriesCount >= MaxRetries)
                {
                    mapeador.Desactivar(user.Id);
                    return new LoginResult { Success = false, Message = "Usuario bloqueado por exceso de intentos fallidos. Contacte al administrador." };
                }
                else
                {
                    mapeador.ActualizarRetries(user.Id, user.RetriesCount);
                    int retriesLeft = MaxRetries - user.RetriesCount;
                    return new LoginResult { Success = false, Message = $"Usuario o contraseña incorrectos. Intentos restantes: {retriesLeft}" };
                }
            }
        }

        public bool Logout(UserBE user)
        {
            return mapeador.Guardar(user);
        }
        #endregion
    }
}
