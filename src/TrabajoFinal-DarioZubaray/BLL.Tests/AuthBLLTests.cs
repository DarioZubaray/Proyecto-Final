using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class AuthBLLTests
    {
        #region Propiedades
        private Mock<IUserMPP> _mockUserMPP;
        private Mock<ITwoFactorBLL> _mockTwoFactor;
        private AuthBLL _authBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockUserMPP = new Mock<IUserMPP>();
            _mockTwoFactor = new Mock<ITwoFactorBLL>();
            _authBLL = new AuthBLL(_mockUserMPP.Object, _mockTwoFactor.Object);
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
        public void Login_AdminInactive_ReturnsSuccess()
        {
            string password = "admin";
            var admin = new UserBE
            {
                Id = 1,
                UserName = "admin",
                IsActive = false,
                RetriesCount = 3,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("admin"))
                .Returns(admin);

            var result = _authBLL.Login("admin", password);

            Assert.IsTrue(result.Success);
            _mockUserMPP.Verify(m => m.Deactivate(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void Login_AdminInactive_IgnoresUserNameCase()
        {
            string password = "admin";
            var admin = new UserBE
            {
                Id = 1,
                UserName = "Admin",
                IsActive = false,
                RetriesCount = 0,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("Admin"))
                .Returns(admin);

            var result = _authBLL.Login("Admin", password);

            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void Login_AdminWrongPassword_DoesNotCountRetriesNorDeactivates()
        {
            var admin = new UserBE
            {
                Id = 1,
                UserName = "admin",
                IsActive = true,
                RetriesCount = 2,
                PasswordHash = EncryptionBLL.HashPassword("correctpassword")
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("admin"))
                .Returns(admin);

            var result = _authBLL.Login("admin", "wrongpassword");

            Assert.IsFalse(result.Success);
            Assert.AreEqual(Resources.Auth_InvalidCredentials, result.Message);
            Assert.AreEqual(ErrorCodesBLL.Auth.InvalidCredentials, result.ErrorCode);
            _mockUserMPP.Verify(m => m.UpdateRetries(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _mockUserMPP.Verify(m => m.Deactivate(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(2, admin.RetriesCount);
        }

        [TestMethod]
        public void Login_AdminCorrectPassword_RequiresTwoFactor()
        {
            string password = "admin";
            var admin = new UserBE
            {
                Id = 1,
                UserName = "admin",
                IsActive = true,
                RetriesCount = 0,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("admin"))
                .Returns(admin);

            var result = _authBLL.Login("admin", password);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.RequiresTwoFactor);
        }

        [TestMethod]
        public void Login_NormalUser_DoesNotRequireTwoFactor()
        {
            string password = "correctpassword";
            var user = new UserBE
            {
                Id = 2,
                UserName = "testuser",
                IsActive = true,
                RetriesCount = 0,
                PasswordHash = EncryptionBLL.HashPassword(password)
            };

            _mockUserMPP
                .Setup(m => m.GetByUserName("testuser"))
                .Returns(user);

            var result = _authBLL.Login("testuser", password);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result.RequiresTwoFactor);
        }

        [TestMethod]
        public void ValidateTwoFactor_ValidCode_ReturnsTrue()
        {
            var admin = new UserBE { Id = 1, UserName = "admin" };
            _mockTwoFactor
                .Setup(m => m.Verify("admin", "123"))
                .Returns(true);

            var result = _authBLL.ValidateTwoFactor(admin, "123");

            Assert.IsTrue(result);
            _mockTwoFactor.Verify(m => m.Verify("admin", "123"), Times.Once);
        }

        [TestMethod]
        public void ValidateTwoFactor_InvalidCode_ReturnsFalse()
        {
            var admin = new UserBE { Id = 1, UserName = "admin" };
            _mockTwoFactor
                .Setup(m => m.Verify("admin", "wrong"))
                .Returns(false);

            var result = _authBLL.ValidateTwoFactor(admin, "wrong");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoFactor_NullUserOrEmptyCode_ReturnsFalse()
        {
            var admin = new UserBE { Id = 1, UserName = "admin" };

            Assert.IsFalse(_authBLL.ValidateTwoFactor(null, "123"));
            Assert.IsFalse(_authBLL.ValidateTwoFactor(admin, ""));
            Assert.IsFalse(_authBLL.ValidateTwoFactor(admin, null));
            _mockTwoFactor.Verify(m => m.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

        [TestMethod]
        public void TestConnection_ReturnsTrue()
        {
            _mockUserMPP
                .Setup(m => m.TestConnection())
                .Returns(true);

            Assert.IsTrue(_authBLL.TestConnection());
            _mockUserMPP.Verify(m => m.TestConnection(), Times.Once);
        }

        [TestMethod]
        public void TestConnection_ReturnsFalse()
        {
            _mockUserMPP
                .Setup(m => m.TestConnection())
                .Returns(false);

            Assert.IsFalse(_authBLL.TestConnection());
        }
        #endregion
    }
}
