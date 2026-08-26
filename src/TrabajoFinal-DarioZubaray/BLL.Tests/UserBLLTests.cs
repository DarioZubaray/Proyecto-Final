using System.Collections.Generic;

using BE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #endregion
    }
}
