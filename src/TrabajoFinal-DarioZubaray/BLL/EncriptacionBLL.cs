using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL
{
    public static class EncriptacionBLL
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (EsHashLegacy(hashedPassword))
            {
                return VerificarHashLegacy(password, hashedPassword);
            }

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        private static bool EsHashLegacy(string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || hashedPassword.Length != 64)
            {
                return false;
            }

            return EsHexadecimal(hashedPassword);
        }

        private static bool EsHexadecimal(string value)
        {
            foreach (char c in value)
            {
                if (!Uri.IsHexDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool VerificarHashLegacy(string password, string storedHash)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString() == storedHash;
            }
        }
    }
}
