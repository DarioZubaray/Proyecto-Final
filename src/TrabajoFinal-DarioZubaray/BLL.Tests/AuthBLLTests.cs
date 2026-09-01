using System;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MPP;

namespace BLL.Tests
{
    [TestClass]
    public class AuthBLLTests
    {
        #region Propiedades
        private Mock<IUserMPP> _mockUserMPP;
        private AuthBLL _authBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockUserMPP = new Mock<IUserMPP>();
            _authBLL = new AuthBLL(_mockUserMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void Login_CredentialsEmpty_ReturnsFailed()
        {
            var result = _authBLL.Login("", "");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_RequiredFields, result.Message);
        }

        [TestMethod]
        public void Login_UserNameEmpty_ReturnsFailed()
        {
            var result = _authBLL.Login("", "password123");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_RequiredFields, result.Message);
        }

        [TestMethod]
        public void Login_PasswordEmpty_ReturnsFailed()
        {
            var result = _authBLL.Login("admin", "");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_RequiredFields, result.Message);
        }

        [TestMethod]
        public void Login_UserNotFound_ReturnsFailed()
        {
            _mockUserMPP
                .Setup(m => m.GetByUserName("nonexistent"))
                .Returns((UserBE)null);

            var result = _authBLL.Login("nonexistent", "password123");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_InvalidCredentials, result.Message);
        }

        [TestMethod]
        public void Login_UserInactive_ReturnsFailed()
        {
            var inactiveUser = new UserBE
            {
                Id = 1,
                UserName = "inactive",
                IsActive = false,
                RetriesCount = 0
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("inactive"))
                .Returns(inactiveUser);

            var result = _authBLL.Login("inactive", "password123");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_UserBlocked, result.Message);
        }

        [TestMethod]
        public void Login_WrongPassword_IncrementsRetries()
        {
            var user = new UserBE
            {
                Id = 1,
                UserName = "testuser",
                IsActive = true,
                RetriesCount = 1,
                PasswordHash = EncryptionBLL.HashPassword("correctpassword")
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("testuser"))
                .Returns(user);

            var result = _authBLL.Login("testuser", "wrongpassword");

            Assert.IsFalse(result.Success);
            _mockUserMPP.Verify(m => m.UpdateRetries(user.Id, 2), Times.Once);
        }

        [TestMethod]
        public void Login_WrongPassword3Times_DeactivatesUser()
        {
            var user = new UserBE
            {
                Id = 1,
                UserName = "testuser",
                IsActive = true,
                RetriesCount = 2,
                PasswordHash = EncryptionBLL.HashPassword("correctpassword")
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("testuser"))
                .Returns(user);

            var result = _authBLL.Login("testuser", "wrongpassword");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_MaxRetriesExceeded, result.Message);
            _mockUserMPP.Verify(m => m.Deactivate(user.Id), Times.Once);
        }

        [TestMethod]
        public void Login_CorrectPassword_ReturnsSuccess()
        {
            string password = "correctpassword";
            var user = new UserBE
            {
                Id = 1,
                UserName = "admin",
                IsActive = true,
                RetriesCount = 0,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("admin"))
                .Returns(user);

            var result = _authBLL.Login("admin", password);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(Resources.Auth_LoginSuccess, result.Message);
            Assert.AreEqual(user, result.User);
            _mockUserMPP.Verify(m => m.UpdateLastUpdate(user.Id, It.IsAny<DateTime>()), Times.Once);
        }

        [TestMethod]
        public void Login_CorrectPassword_ResetsRetries()
        {
            string password = "correctpassword";
            var user = new UserBE
            {
                Id = 1,
                UserName = "admin",
                IsActive = true,
                RetriesCount = 2,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("admin"))
                .Returns(user);

            var result = _authBLL.Login("admin", password);

            Assert.IsTrue(result.Success);
            _mockUserMPP.Verify(m => m.UpdateRetries(user.Id, 0), Times.Once);
        }

        [TestMethod]
        public void Logout_SavesUser_ReturnsTrue()
        {
            var user = new UserBE { Id = 1, UserName = "admin" };
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _authBLL.Logout(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.Save(user), Times.Once);
        }
        #endregion
    }
}
