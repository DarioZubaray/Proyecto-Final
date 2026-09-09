using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Composite;
using BE.Entities;
using BLL.Services;
using Moq;
using MPP;

namespace BLL.Tests
{
    [TestClass]
    public class PermissionBLLTests
    {
        #region Propiedades
        private Mock<IRoleMPP> _mockRoleMPP;
        private PermissionBLL _permissionBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockRoleMPP = new Mock<IRoleMPP>();
            _permissionBLL = new PermissionBLL(_mockRoleMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void BuildRoleTree_ExistingRole_IncludesPermissionsAndChildRoles()
        {
            var admin = new RoleBE(1, "Admin", new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)
            });
            var supervisor = new RoleBE(2, "Supervisor", new List<PermissionBE>
            {
                new PermissionBE(2, "FORM_COMPLAINTS", "Reclamos", null)
            });

            _mockRoleMPP.Setup(m => m.FindById(1)).Returns(admin);
            _mockRoleMPP.Setup(m => m.GetChildRoleIds(1)).Returns(new List<int> { 2 });
            _mockRoleMPP.Setup(m => m.FindById(2)).Returns(supervisor);

            RoleCompositeBE tree = _permissionBLL.BuildRoleTree(1);

            Assert.IsNotNull(tree);
            Assert.AreEqual(1, tree.Id);
            Assert.AreEqual("Admin", tree.Name);
            Assert.IsTrue(tree.HasPermission("FORM_USER_MGMT"));
            Assert.IsTrue(tree.HasPermission("FORM_COMPLAINTS"));
            Assert.IsFalse(tree.HasPermission("FORM_REPORTS"));
        }

        [TestMethod]
        public void BuildRoleTree_NonExistingRole_ReturnsNull()
        {
            _mockRoleMPP.Setup(m => m.FindById(99)).Returns((RoleBE)null);

            RoleCompositeBE tree = _permissionBLL.BuildRoleTree(99);

            Assert.IsNull(tree);
        }

        [TestMethod]
        public void BuildRoleTree_ChildRoleMissing_IsSkipped()
        {
            var admin = new RoleBE(1, "Admin", new List<PermissionBE>
            {
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)
            });

            _mockRoleMPP.Setup(m => m.FindById(1)).Returns(admin);
            _mockRoleMPP.Setup(m => m.GetChildRoleIds(1)).Returns(new List<int> { 5 });
            _mockRoleMPP.Setup(m => m.FindById(5)).Returns((RoleBE)null);

            RoleCompositeBE tree = _permissionBLL.BuildRoleTree(1);

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.HasPermission("FORM_USER_MGMT"));
        }

        [TestMethod]
        public void BuildRoleTree_NoChildren_ReturnsOwnPermissions()
        {
            var operatorRole = new RoleBE(3, "Operador", new List<PermissionBE>
            {
                new PermissionBE(3, "FORM_REPORTS", "Reportes", null)
            });

            _mockRoleMPP.Setup(m => m.FindById(3)).Returns(operatorRole);
            _mockRoleMPP.Setup(m => m.GetChildRoleIds(3)).Returns(new List<int>());

            RoleCompositeBE tree = _permissionBLL.BuildRoleTree(3);

            Assert.IsNotNull(tree);
            Assert.IsTrue(tree.HasPermission("FORM_REPORTS"));
            Assert.IsFalse(tree.HasPermission("FORM_USER_MGMT"));
        }

        [TestMethod]
        public void HasPermission_NullTree_ReturnsFalse()
        {
            Assert.IsFalse(_permissionBLL.HasPermission(null, "FORM_USER_MGMT"));
        }

        [TestMethod]
        public void HasPermission_TreeWithPermission_ReturnsTrue()
        {
            var composite = new RoleCompositeBE(1, "Admin");
            composite.AddChild(new PermissionLeafBE(
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)));

            Assert.IsTrue(_permissionBLL.HasPermission(composite, "FORM_USER_MGMT"));
        }

        [TestMethod]
        public void HasPermission_TreeWithoutPermission_ReturnsFalse()
        {
            var composite = new RoleCompositeBE(1, "Admin");
            composite.AddChild(new PermissionLeafBE(
                new PermissionBE(1, "FORM_USER_MGMT", "Usuarios", null)));

            Assert.IsFalse(_permissionBLL.HasPermission(composite, "FORM_REPORTS"));
        }
        #endregion
    }
}