using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using Moq;
using MPP;

namespace BLL.Tests
{
    [TestClass]
    public class RoleBLLTests
    {
        #region Propiedades
        private Mock<IRoleMPP> _mockRoleMPP;
        private RoleBLL _roleBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockRoleMPP = new Mock<IRoleMPP>();
            _roleBLL = new RoleBLL(_mockRoleMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var expected = new RoleBE { Id = 1, Name = "Admin" };
            _mockRoleMPP.Setup(m => m.FindById(1)).Returns(expected);

            RoleBE result = _roleBLL.FindById(1);

            Assert.AreEqual(expected, result);
            _mockRoleMPP.Verify(m => m.FindById(1), Times.Once);
        }

        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expected = new List<RoleBE>
            {
                new RoleBE { Id = 1, Name = "Admin" },
                new RoleBE { Id = 2, Name = "Profesor" }
            };
            _mockRoleMPP.Setup(m => m.FindAll()).Returns(expected);

            List<RoleBE> result = _roleBLL.FindAll();

            CollectionAssert.AreEqual(expected, result);
            _mockRoleMPP.Verify(m => m.FindAll(), Times.Once);
        }

        [TestMethod]
        public void GetPermissionsByRoleId_DelegatesToMPP()
        {
            var expected = new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)
            };
            _mockRoleMPP.Setup(m => m.GetPermissionsByRoleId(5)).Returns(expected);

            List<PermissionBE> result = _roleBLL.GetPermissionsByRoleId(5);

            CollectionAssert.AreEqual(expected, result);
            _mockRoleMPP.Verify(m => m.GetPermissionsByRoleId(5), Times.Once);
        }

        [TestMethod]
        public void GetChildRoleIds_DelegatesToMPP()
        {
            _mockRoleMPP.Setup(m => m.GetChildRoleIds(1)).Returns(new List<int> { 2 });

            List<int> result = _roleBLL.GetChildRoleIds(1);

            CollectionAssert.AreEqual(new List<int> { 2 }, result);
            _mockRoleMPP.Verify(m => m.GetChildRoleIds(1), Times.Once);
        }

        [TestMethod]
        public void Save_DelegatesToMPP()
        {
            var role = new RoleBE { Id = 0, Name = "Nuevo" };
            _mockRoleMPP.Setup(m => m.Save(role)).Returns(7);

            int result = _roleBLL.Save(role);

            Assert.AreEqual(7, result);
            _mockRoleMPP.Verify(m => m.Save(role), Times.Once);
        }

        [TestMethod]
        public void Delete_DelegatesToMPP()
        {
            _mockRoleMPP.Setup(m => m.Delete(3)).Returns(true);

            bool result = _roleBLL.Delete(3);

            Assert.IsTrue(result);
            _mockRoleMPP.Verify(m => m.Delete(3), Times.Once);
        }

        [TestMethod]
        public void GetAllPermissions_DelegatesToMPP()
        {
            var expected = new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)
            };
            _mockRoleMPP.Setup(m => m.GetAllPermissions()).Returns(expected);

            List<PermissionBE> result = _roleBLL.GetAllPermissions();

            CollectionAssert.AreEqual(expected, result);
            _mockRoleMPP.Verify(m => m.GetAllPermissions(), Times.Once);
        }

        [TestMethod]
        public void SavePermissions_PreservesSystemPermissions()
        {
            var currentPerms = new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null, isSystem: true),
                new PermissionBE(2, "FORM_ROLE_MGMT", "Roles", null, isSystem: true),
                new PermissionBE(3, "FORM_COMPLAINTS", "Reclamos", null, isSystem: false)
            };
            _mockRoleMPP.Setup(m => m.GetPermissionsByRoleId(10)).Returns(currentPerms);

            var permissionIds = new List<int> { 3, 4 };

            _roleBLL.SavePermissions(10, permissionIds);

            _mockRoleMPP.Verify(m => m.SavePermissions(10, It.Is<List<int>>(l =>
                l.Count == 4 &&
                l.Contains(1) &&
                l.Contains(2) &&
                l.Contains(3) &&
                l.Contains(4))), Times.Once);
        }

        [TestMethod]
        public void SavePermissions_SystemPermissionsAlreadyIncluded_NoDuplicates()
        {
            var currentPerms = new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null, isSystem: true)
            };
            _mockRoleMPP.Setup(m => m.GetPermissionsByRoleId(10)).Returns(currentPerms);

            var permissionIds = new List<int> { 1, 2 };

            _roleBLL.SavePermissions(10, permissionIds);

            _mockRoleMPP.Verify(m => m.SavePermissions(10, It.Is<List<int>>(l =>
                l.Count == 2 &&
                l.FindAll(id => id == 1).Count == 1 &&
                l.Contains(2))), Times.Once);
        }

        [TestMethod]
        public void SavePermissions_NoSystemPermissions_AssignsAsIs()
        {
            var currentPerms = new List<PermissionBE>
            {
                new PermissionBE(3, "FORM_COMPLAINTS", "Reclamos", null, isSystem: false)
            };
            _mockRoleMPP.Setup(m => m.GetPermissionsByRoleId(10)).Returns(currentPerms);

            var permissionIds = new List<int> { 4, 5 };

            _roleBLL.SavePermissions(10, permissionIds);

            _mockRoleMPP.Verify(m => m.SavePermissions(10, It.Is<List<int>>(l =>
                l.Count == 2 && l[0] == 4 && l[1] == 5)), Times.Once);
        }

        [TestMethod]
        public void SavePermissions_NoCurrentPermissions_PassesThrough()
        {
            _mockRoleMPP.Setup(m => m.GetPermissionsByRoleId(10)).Returns(new List<PermissionBE>());

            var permissionIds = new List<int> { 1, 2 };

            _roleBLL.SavePermissions(10, permissionIds);

            _mockRoleMPP.Verify(m => m.SavePermissions(10, It.Is<List<int>>(l =>
                l.Count == 2)), Times.Once);
        }
        #endregion
    }
}