using System;

namespace BLL.Strategy
{
    public class BcryptPasswordStrategy : IPasswordStrategy
    {
        public bool Matches(string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
            {
                return false;
            }

            return !IsLegacyHash(storedHash);
        }

        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string plain, string stored)
        {
            return BCrypt.Net.BCrypt.Verify(plain, stored);
        }

        private bool IsLegacyHash(string storedHash)
        {
            return storedHash.Length == 64 && IsHexadecimal(storedHash);
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
