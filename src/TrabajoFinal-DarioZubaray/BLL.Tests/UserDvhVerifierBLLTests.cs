using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.DTOs;
using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;
using Moq;

namespace BLL.Tests
{
    [TestClass]
    public class UserDvhVerifierBLLTests
    {
        #region Propiedades
        private Mock<IUserBLL> _mockUserBLL;
        private UserDvhVerifierBLL _verifier;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockUserBLL = new Mock<IUserBLL>();
            _verifier = new UserDvhVerifierBLL(_mockUserBLL.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TableName_ReturnsUsers()
        {
            Assert.AreEqual("users", _verifier.TableName);
        }

        [TestMethod]
        public void FindViolations_UserWithValidDvh_ReturnsEmpty()
        {
            UserBE user = CreateUser();
            user.Dvh = DVHHelperBLL.Compute(user.GetDvhValues());

            _mockUserBLL.Setup(m => m.FindAll()).Returns(new List<UserBE> { user });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(0, violations.Count);
        }

        [TestMethod]
        public void FindViolations_UserWithAlteredDvh_ReturnsViolation()
        {
            UserBE user = CreateUser();
            user.Dvh = "hash-viejo-que-no-coincide";

            _mockUserBLL.Setup(m => m.FindAll()).Returns(new List<UserBE> { user });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual("users", violations[0].TableName);
            Assert.AreEqual(user.Id, violations[0].RowId);
            Assert.AreEqual(user.UserName, violations[0].Description);
            Assert.AreEqual("hash-viejo-que-no-coincide", violations[0].StoredHash);
            Assert.AreEqual(DVHHelperBLL.Compute(user.GetDvhValues()), violations[0].ComputedHash);
        }

        [TestMethod]
        public void FindViolations_UserWithoutDvh_ReturnsViolation()
        {
            UserBE user = CreateUser();
            user.Dvh = null;

            _mockUserBLL.Setup(m => m.FindAll()).Returns(new List<UserBE> { user });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.IsNull(violations[0].StoredHash);
        }

        [TestMethod]
        public void AcceptAndRegenerate_UserFound_SavesAndRegenerates()
        {
            UserBE user = CreateUser();
            user.Dvh = "hash-alterado";

            _mockUserBLL
                .Setup(m => m.FindById(It.Is<UserBE>(u => u.Id == user.Id)))
                .Returns(user);
            _mockUserBLL.Setup(m => m.Save(user)).Returns(true);

            var violation = new IntegrityViolationBE
            {
                TableName = "users",
                RowId = user.Id
            };

            bool result = _verifier.AcceptAndRegenerate(violation);

            Assert.IsTrue(result);
            _mockUserBLL.Verify(m => m.Save(user), Times.Once);
        }

        [TestMethod]
        public void AcceptAndRegenerate_UserNotFound_ReturnsFalse()
        {
            _mockUserBLL
                .Setup(m => m.FindById(It.IsAny<UserBE>()))
                .Returns((UserBE)null);

            var violation = new IntegrityViolationBE { TableName = "users", RowId = 99 };

            Assert.IsFalse(_verifier.AcceptAndRegenerate(violation));
            _mockUserBLL.Verify(m => m.Save(It.IsAny<UserBE>()), Times.Never);
        }

        [TestMethod]
        public void AcceptAndRegenerate_NullOrInvalid_ReturnsFalse()
        {
            Assert.IsFalse(_verifier.AcceptAndRegenerate(null));
            Assert.IsFalse(_verifier.AcceptAndRegenerate(new IntegrityViolationBE { TableName = "users", RowId = 0 }));
        }
        #endregion

        #region Helpers
        private UserBE CreateUser()
        {
            return new UserBE
            {
                Id = 3,
                UserName = "usuario_prueba",
                PasswordHash = "hash_password",
                IsActive = true,
                RetriesCount = 0,
                LastUpdate = new System.DateTime(2026, 10, 9, 10, 0, 0),
                CreatedAt = new System.DateTime(2026, 1, 1, 10, 0, 0),
                RoleId = 1,
                Language = "es",
                Theme = "System"
            };
        }
        #endregion
    }
}
