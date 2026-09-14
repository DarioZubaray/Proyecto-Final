using BLL.Strategy;

namespace BLL.Helpers
{
    public static class EncryptionBLL
    {
        #region Métodos
        public static string HashPassword(string password)
        {
            return PasswordHasherBLL.Default.Hash(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHasherBLL.Default.Verify(password, hashedPassword);
        }
        #endregion
    }
}
