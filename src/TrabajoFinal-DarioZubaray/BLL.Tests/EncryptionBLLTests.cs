using System;
using BLL.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    [TestClass]
    public class EncryptionBLLTests
    {
        #region Tests
        [TestMethod]
        public void HashPassword_ReturnsNonEmptyHash()
        {
            string password = "mypassword123";

            string hash = EncryptionBLL.HashPassword(password);

            Assert.IsFalse(string.IsNullOrEmpty(hash));
            Assert.AreNotEqual(password, hash);
        }

        [TestMethod]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            string password = "mypassword123";
            string hash = EncryptionBLL.HashPassword(password);

            bool result = EncryptionBLL.VerifyPassword(password, hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_WrongPassword_ReturnsFalse()
        {
            string password = "mypassword123";
            string hash = EncryptionBLL.HashPassword(password);

            bool result = EncryptionBLL.VerifyPassword("wrongpassword", hash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_LegacyHash_WorksCorrectly()
        {
            // Generar un hash SHA256 legacy conocido
            string password = "password123";
            string legacyHash = ComputeSHA256(password);

            bool result = EncryptionBLL.VerifyPassword(password, legacyHash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_NullHash_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                EncryptionBLL.VerifyPassword("password", null));
        }

        [TestMethod]
        public void VerifyPassword_EmptyHash_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                EncryptionBLL.VerifyPassword("password", ""));
        }
        #endregion

        #region Métodos Auxiliares
        private string ComputeSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var builder = new System.Text.StringBuilder();
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
