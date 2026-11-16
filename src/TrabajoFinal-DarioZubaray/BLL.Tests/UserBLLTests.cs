using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using BLL.Services;
using Moq;
using MPP.Interfaces;

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
            _mockUserMPP
                .Setup(m => m.UpdateDvh(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);
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
        public void Save_RegeneratesDvh()
        {
            var user = new UserBE { Id = 5, UserName = "newuser" };
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(
                m => m.UpdateDvh(user.Id, DVHHelperBLL.Compute(user.GetDvhValues())),
                Times.Once);
        }

        [TestMethod]
        public void UpdateLanguage_RegeneratesDvh()
        {
            var persisted = new UserBE { Id = 3, UserName = "user3", Language = "es" };
            _mockUserMPP
                .Setup(m => m.UpdateLanguage(3, "en"))
                .Returns(true);
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 3)))
                .Returns(persisted);

            var result = _userBLL.UpdateLanguage(3, "en");

            Assert.IsTrue(result);
            _mockUserMPP.Verify(
                m => m.UpdateDvh(3, DVHHelperBLL.Compute(persisted.GetDvhValues())),
                Times.Once);
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

        [TestMethod]
        public void Delete_Admin_ReturnsFalseWithoutPersisting()
        {
            var admin = new UserBE { Id = 1, UserName = "admin" };

            var result = _userBLL.Delete(admin);

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.Delete(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void Delete_Admin_IgnoresUserNameCase()
        {
            var admin = new UserBE { Id = 1, UserName = "Admin" };

            var result = _userBLL.Delete(admin);

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.Delete(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_Admin_ForcesActiveAndZeroRetries()
        {
            var admin = new UserBE
            {
                Id = 1,
                UserName = "admin",
                RoleId = 1,
                IsActive = false,
                RetriesCount = 3
            };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(new UserBE { Id = 1, UserName = "admin", RoleId = 1, IsActive = true });
            _mockUserMPP
                .Setup(m => m.Save(admin))
                .Returns(true);

            var result = _userBLL.Save(admin);

            Assert.IsTrue(result);
            Assert.IsTrue(admin.IsActive);
            Assert.AreEqual(0, admin.RetriesCount);
            _mockUserMPP.Verify(m => m.Save(admin), Times.Once);
        }

        [TestMethod]
        public void Save_AdminRoleChange_BlockedReturnsFalseWithoutPersisting()
        {
            var admin = new UserBE
            {
                Id = 1,
                UserName = "admin",
                RoleId = 2
            };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(new UserBE { Id = 1, UserName = "admin", RoleId = 1 });

            var result = _userBLL.Save(admin);

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.Save(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_NormalUserRoleChange_Allowed()
        {
            var user = new UserBE { Id = 2, UserName = "dario", RoleId = 4 };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 2)))
                .Returns(new UserBE { Id = 2, UserName = "dario", RoleId = 2 });
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.Save(user), Times.Once);
        }

        [TestMethod]
        public void Save_AdminRename_BlockedReturnsFalseWithoutPersisting()
        {
            var admin = new UserBE
            {
                Id = 1,
                UserName = "nuevoNombre",
                IsActive = true,
                RetriesCount = 0
            };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(new UserBE { Id = 1, UserName = "admin", IsActive = true });

            var result = _userBLL.Save(admin);

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.Save(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_AdminRename_IgnoresUserNameCase()
        {
            var admin = new UserBE { Id = 1, UserName = "pepe" };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 1)))
                .Returns(new UserBE { Id = 1, UserName = "Admin" });

            var result = _userBLL.Save(admin);

            Assert.IsFalse(result);
            _mockUserMPP.Verify(m => m.Save(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_NormalUserRename_Allowed()
        {
            var user = new UserBE { Id = 2, UserName = "nombreNuevo" };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 2)))
                .Returns(new UserBE { Id = 2, UserName = "nombreViejo" });
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            _mockUserMPP.Verify(m => m.Save(user), Times.Once);
        }

        [TestMethod]
        public void IsAdminName_ComparesCaseInsensitive()
        {
            Assert.IsTrue(UserBLL.IsAdminName("admin"));
            Assert.IsTrue(UserBLL.IsAdminName("Admin"));
            Assert.IsTrue(UserBLL.IsAdminName("ADMIN"));
            Assert.IsFalse(UserBLL.IsAdminName("dario"));
            Assert.IsFalse(UserBLL.IsAdminName(""));
            Assert.IsFalse(UserBLL.IsAdminName(null));
        }

        [TestMethod]
        public void Save_NormalUser_KeepsInactiveState()
        {
            var user = new UserBE
            {
                Id = 2,
                UserName = "userToDelete",
                IsActive = false,
                RetriesCount = 3
            };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 2)))
                .Returns(new UserBE { Id = 2, UserName = "userToDelete" });
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            Assert.IsFalse(user.IsActive);
            Assert.AreEqual(3, user.RetriesCount);
        }

        [TestMethod]
        public void Save_Reactivation_ResetsRetries()
        {
            var user = new UserBE
            {
                Id = 2,
                UserName = "blockedUser",
                IsActive = true,
                RetriesCount = 3
            };
            _mockUserMPP
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == 2)))
                .Returns(new UserBE { Id = 2, UserName = "blockedUser", IsActive = false, RetriesCount = 3 });
            _mockUserMPP
                .Setup(m => m.Save(user))
                .Returns(true);

            var result = _userBLL.Save(user);

            Assert.IsTrue(result);
            Assert.IsTrue(user.IsActive);
            Assert.AreEqual(0, user.RetriesCount);
            _mockUserMPP.Verify(m => m.Save(user), Times.Once);
        }

        [TestMethod]
        public void HasRegisteredUsers_WhenOtherUsersExist_ReturnsTrue()
        {
            _mockUserMPP
                .Setup(m => m.CountExceptUserName(UserBLL.ADMIN_USERNAME))
                .Returns(3);

            Assert.IsTrue(_userBLL.HasRegisteredUsers());
            _mockUserMPP.Verify(m => m.CountExceptUserName(UserBLL.ADMIN_USERNAME), Times.Once);
        }

        [TestMethod]
        public void HasRegisteredUsers_WhenOnlyDefaultAdmin_ReturnsFalse()
        {
            _mockUserMPP
                .Setup(m => m.CountExceptUserName(UserBLL.ADMIN_USERNAME))
                .Returns(0);

            Assert.IsFalse(_userBLL.HasRegisteredUsers());
        }
        #endregion
    }
}
