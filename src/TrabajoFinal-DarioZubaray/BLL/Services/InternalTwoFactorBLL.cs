using BLL.Interfaces;

namespace BLL.Services
{
    // Implementacion interna hardcodeada del 2FA: codigo valido = "123".
    // TODO: reemplazar por implementaciones reales (Google/Microsoft, etc.)
    // que cumplan ITwoFactorBLL, configurandolas en ServiceLocatorBLL.CreateTwoFactorBLL().
    public class InternalTwoFactorBLL : ITwoFactorBLL
    {
        #region Constantes
        private const string ValidCode = "123";
        #endregion

        #region Propiedades
        public string ProviderName => "Internal";
        #endregion

        #region Métodos
        public bool Verify(string userName, string code)
        {
            return code == ValidCode;
        }
        #endregion
    }
}
