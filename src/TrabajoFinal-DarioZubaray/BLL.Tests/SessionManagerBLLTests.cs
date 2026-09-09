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
    public class SessionManagerBLLTests
    {
        #region Propiedades
        private const int TestUserId = 1;
        #endregion

        #region Inicialización
        [TestCleanup]
        public void Cleanup()
        {
            SessionManagerBLL.RemoveSession(TestUserId);
            CultureHelperBLL.SetCulture(CultureHelperBLL.DefaultLanguage);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void CreateSession_NullUser_ReturnsNull()
        {
            var session = SessionManagerBLL.CreateSession(null);

            Assert.IsNull(session);
        }

        [TestMethod]
        public void CreateSession_RegistersSessionInInstanceStore()
        {
            var session = CreateAdminSession();

            Assert.IsNotNull(session);
            Assert.AreSame(session, SessionManagerBLL.GetInstance(TestUserId));
        }

        [TestMethod]
        public void GetInstance_UnknownUser_ReturnsNull()
        {
            Assert.IsNull(SessionManagerBLL.GetInstance(999));
        }

        [TestMethod]
        public void RemoveSession_RemovesFromInstanceStore()
        {
            SessionManagerBLL.CreateSession(CreateAdminUser(), CreatePermissionBLLWithAdmin());

            SessionManagerBLL.RemoveSession(TestUserId);

            Assert.IsNull(SessionManagerBLL.GetInstance(TestUserId));
        }

        [TestMethod]
        public void UpdateLanguage_UpdatesUserAndCulture()
        {
            var session = CreateAdminSession();

            session.UpdateLanguage("pt-BR");

            Assert.AreEqual("pt-BR", session.User.Language);
            Assert.AreEqual("pt-BR", System.Threading.Thread.CurrentThread.CurrentCulture.Name);
        }

        [TestMethod]
        public void UpdateTheme_UpdatesUserTheme()
        {
            var session = CreateAdminSession();

            session.UpdateTheme("Light");

            Assert.AreEqual("Light", session.User.Theme);
        }

        [TestMethod]
        public void HasPermission_WithMatchingPermission_ReturnsTrue()
        {
            var session = CreateAdminSession();

            Assert.IsTrue(session.HasPermission("FORM_USER_MGMT"));
        }

        [TestMethod]
        public void HasPermission_WithoutMatchingPermission_ReturnsFalse()
        {
            var session = CreateAdminSession();

            Assert.IsFalse(session.HasPermission("FORM_REPORTS"));
        }

        [TestMethod]
        public void HasPermission_NullRoleTree_ReturnsFalse()
        {
            var user = new UserBE
            {
                Id = TestUserId,
                UserName = "sinrol",
                Language = "es",
                RoleId = 0
            };

            var mock = new Mock<IRoleMPP>();
            mock.Setup(m => m.FindById(0)).Returns((RoleBE)null);
            mock.Setup(m => m.GetChildRoleIds(0)).Returns(new List<int>());
            var permissionBLL = new PermissionBLL(mock.Object);

            var session = SessionManagerBLL.CreateSession(user, permissionBLL);

            Assert.IsNotNull(session);
            Assert.IsFalse(session.HasPermission("FORM_USER_MGMT"));
        }
        #endregion

        #region Métodos Auxiliares
        private static UserBE CreateAdminUser()
        {
            return new UserBE
            {
                Id = TestUserId,
                UserName = "admin",
                Language = "es",
                Theme = "Dark",
                RoleId = 1
            };
        }

        private static PermissionBLL CreatePermissionBLLWithAdmin()
        {
            var role = new RoleBE(1, "Admin", new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)
            });

            var mock = new Mock<IRoleMPP>();
            mock.Setup(m => m.FindById(1)).Returns(role);
            mock.Setup(m => m.GetChildRoleIds(1)).Returns(new List<int>());

            return new PermissionBLL(mock.Object);
        }

        private static SessionManagerBLL CreateAdminSession()
        {
            return SessionManagerBLL.CreateSession(
                CreateAdminUser(), CreatePermissionBLLWithAdmin());
        }
        #endregion
    }
}
