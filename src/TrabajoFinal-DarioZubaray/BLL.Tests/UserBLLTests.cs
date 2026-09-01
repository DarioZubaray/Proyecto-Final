using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using BLL.Services;
using Moq;
using MPP;

namespace BLL.Tests
{
    [TestClass]
    public class UserBLLTests
    {
        #region Propiedades
        private Mock<IUserMPP> _mockUserMPP;
        private UserBLL _userBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockUserMPP = new Mock<IUserMPP>();
            _userBLL = new UserBLL(_mockUserMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void ChangePassword_UserNotFound_ReturnsFalse()
        {
            _mockUserMPP
                .Setup(m => m.FindById(It.IsAny<UserBE>()))
                .Returns((UserBE)null);

            var result = _userBLL.ChangePassword(1, "oldpass", "newpass");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ChangePassword_WrongCurrentPassword_ReturnsFalse()
        {
            var user = new UserBE
            {
                Id = 1,
                UserName = "testuser",
                PasswordHash = EncryptionBLL.HashPassword("correctpassword")
            };

            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(user);

            var result = _userBLL.ChangePassword(1, "wrongpassword", "newpass");

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.UpdatePassword(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ChangePassword_ValidPasswords_ReturnsTrue()
        {
            var user = new UserBE
            {
                Id = 1,
                UserName = "testuser",
                PasswordHash = EncryptionBLL.HashPassword("correctpassword")
            };

            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(user);
            _mockUserMPP
                .Setup(m => m.UpdatePassword(1, It.IsAny<string>()))
                .Returns(true);

            var result = _userBLL.ChangePassword(1, "correctpassword", "newpassword123");

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.UpdatePassword(1, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expectedUsers = new List<UserBE>
            {
                new UserBE { Id = 1, UserName = "user1" },
                new UserBE { Id = 2, UserName = "user2" }
            };

            _mockUserMPP
                .Setup(m => m.FindAll())
                .Returns(expectedUsers);

            var result = _userBLL.FindAll();

            CollectionAssert.AreEqual(expectedUsers, result);
            _mockUserMPP.Verify(m => m.FindAll(), Times.Once);
        }

        [TestMethod]
        public void Save_DelegatesToMPP()
        {
            var user = new UserBE { Id = 0, UserName = "newuser" };
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.Save(user), Times.Once);
        }

        [TestMethod]
        public void Delete_DelegatesToMPP()
        {
            var user = new UserBE { Id = 1, UserName = "userToDelete" };
            _mockUserMPP
                .Setup(m => m.Delete(user))
                .Returns(true);

            var result = _userBLL.Delete(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.Delete(user), Times.Once);
        }

        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var user = new UserBE { Id = 5 };
            var expected = new UserBE { Id = 5, UserName = "found" };

            _mockUserMPP
                .Setup(m => m.FindById(user))
                .Returns(expected);

            var result = _userBLL.FindById(user);

            Assert.AreEqual(expected, result);
            _mockUserMPP.Verify(m => m.FindById(user), Times.Once);
        }

        [TestMethod]
        public void FindByUserName_DelegatesToMPP()
        {
            var expected = new List<UserBE>
            {
                new UserBE { Id = 1, UserName = "testuser" }
            };

            _mockUserMPP
                .Setup(m => m.FindByUserName("test"))
                .Returns(expected);

            var result = _userBLL.FindByUserName("test");

            CollectionAssert.AreEqual(expected, result);
            _mockUserMPP.Verify(m => m.FindByUserName("test"), Times.Once);
        }

        [TestMethod]
        public void UpdateLanguage_DelegatesToMPP()
        {
            _mockUserMPP
                .Setup(m => m.UpdateLanguage(3, "en"))
                .Returns(true);

            var result = _userBLL.UpdateLanguage(3, "en");

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.UpdateLanguage(3, "en"), Times.Once);
        }

        [TestMethod]
        public void UpdateTheme_DelegatesToMPP()
        {
            _mockUserMPP
                .Setup(m => m.UpdateTheme(3, "Dark"))
                .Returns(true);

            var result = _userBLL.UpdateTheme(3, "Dark");

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.UpdateTheme(3, "Dark"), Times.Once);
        }

        [TestMethod]
        public void CountByRoleId_DelegatesToMPP()
        {
            _mockUserMPP
                .Setup(m => m.CountByRoleId(2))
                .Returns(5);

            var result = _userBLL.CountByRoleId(2);

            Assert.AreEqual(5, result);
            _mockUserMPP.Verify(m => m.CountByRoleId(2), Times.Once);
        }
        #endregion
    }
}
