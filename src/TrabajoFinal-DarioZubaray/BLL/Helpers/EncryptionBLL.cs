using BLL.Strategy;

namespace BLL
{
    public static class EncryptionBLL
    {
        #region Métodos
        public static string HashPassword(string password)
        {
            return PasswordHasher.Default.Hash(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHasher.Default.Verify(password, hashedPassword);
        }
        #endregion
    }
}
