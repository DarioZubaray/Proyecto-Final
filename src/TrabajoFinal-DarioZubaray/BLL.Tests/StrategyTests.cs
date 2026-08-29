using System;
using System.Security.Cryptography;
using System.Text;

using BLL.Strategy;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public class StrategyTests
    {
        #region BcryptPasswordStrategy
        [TestMethod]
        public void BcryptStrategy_Verify_CorrectPassword_ReturnsTrue()
        {
            var strategy = new BcryptPasswordStrategy();
            string hash = strategy.Hash("password123");

            bool result = strategy.Verify("password123", hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BcryptStrategy_Verify_WrongPassword_ReturnsFalse()
        {
            var strategy = new BcryptPasswordStrategy();
            string hash = strategy.Hash("password123");

            bool result = strategy.Verify("wrongpassword", hash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void BcryptStrategy_Matches_OnNull_ReturnsFalse()
        {
            var strategy = new BcryptPasswordStrategy();

            Assert.IsFalse(strategy.Matches(null));
        }
        #endregion

        #region LegacySha256PasswordStrategy
        [TestMethod]
        public void LegacyStrategy_Verify_CorrectPassword_ReturnsTrue()
        {
            var strategy = new LegacySha256PasswordStrategy();
            string legacyHash = ComputeSHA256("password123");

            bool result = strategy.Verify("password123", legacyHash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void LegacyStrategy_Verify_WrongPassword_ReturnsFalse()
        {
            var strategy = new LegacySha256PasswordStrategy();
            string legacyHash = ComputeSHA256("password123");

            bool result = strategy.Verify("wrongpassword", legacyHash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LegacyStrategy_Matches_OnLegacyHash_ReturnsTrue()
        {
            var strategy = new LegacySha256PasswordStrategy();
            string legacyHash = ComputeSHA256("password123");

            Assert.IsTrue(strategy.Matches(legacyHash));
        }

        [TestMethod]
        public void LegacyStrategy_Matches_OnBcryptHash_ReturnsFalse()
        {
            var strategy = new LegacySha256PasswordStrategy();
            string bcryptHash = new BcryptPasswordStrategy().Hash("password123");

            Assert.IsFalse(strategy.Matches(bcryptHash));
        }
        #endregion

        #region PasswordHasher (contexto)
        [TestMethod]
        public void PasswordHasher_Hash_IsVerifiableByBcrypt()
        {
            var hasher = new PasswordHasher();

            string hash = hasher.Hash("password123");

            Assert.IsTrue(BCrypt.Net.BCrypt.Verify("password123", hash));
        }

        [TestMethod]
        public void PasswordHasher_Verify_SelectsBcryptForBcryptHash()
        {
            var hasher = new PasswordHasher();
            string hash = hasher.Hash("password123");

            bool result = hasher.Verify("password123", hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PasswordHasher_Verify_SelectsLegacyForLegacyHash()
        {
            var hasher = new PasswordHasher();
            string legacyHash = ComputeSHA256("password123");

            bool result = hasher.Verify("password123", legacyHash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PasswordHasher_Verify_NullHash_ThrowsArgumentNullException()
        {
            var hasher = new PasswordHasher();

            Assert.ThrowsException<ArgumentNullException>(() => hasher.Verify("password", null));
        }

        [TestMethod]
        public void PasswordHasher_Verify_EmptyHash_ThrowsArgumentException()
        {
            var hasher = new PasswordHasher();

            Assert.ThrowsException<ArgumentException>(() => hasher.Verify("password", ""));
        }
        #endregion

        #region Métodos Auxiliares
        private string ComputeSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
    }
}
