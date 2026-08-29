using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Strategy
{
    public class LegacySha256PasswordStrategy : IPasswordStrategy
    {
        public bool Matches(string storedHash)
        {
            return IsLegacyHash(storedHash);
        }

        public string Hash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public bool Verify(string plain, string stored)
        {
            return string.Equals(Hash(plain), stored, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsLegacyHash(string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || storedHash.Length != 64)
            {
                return false;
            }

            return IsHexadecimal(storedHash);
        }

        private bool IsHexadecimal(string value)
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
    }
}
