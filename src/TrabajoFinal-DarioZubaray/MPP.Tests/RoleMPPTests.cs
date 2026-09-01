using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class RoleMPPTests
    {
        private RoleMPP _roleMPP;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _roleMPP = new RoleMPP(_connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        [TestMethod]
        public void FindById_ExistingRole_ReturnsRoleWithPermissions()
        {
            RoleBE role = _roleMPP.FindById(1);

            Assert.IsNotNull(role);
            Assert.AreEqual("Admin", role.Name);
            Assert.AreEqual(4, role.Permissions.Count);
        }

        [TestMethod]
        public void FindById_NonExistingRole_ReturnsNull()
        {
            RoleBE role = _roleMPP.FindById(99999);

            Assert.IsNull(role);
        }

        [TestMethod]
        public void FindAll_ReturnsAllRoles()
        {
            List<RoleBE> roles = _roleMPP.FindAll();

            Assert.IsNotNull(roles);
            Assert.AreEqual(3, roles.Count);
        }

        [TestMethod]
        public void FindAll_EagerLoadsPermissions()
        {
            List<RoleBE> roles = _roleMPP.FindAll();

            foreach (RoleBE role in roles)
            {
                Assert.IsNotNull(role.Permissions);
            }

            RoleBE admin = roles.Find(r => r.Name == "Admin");
            Assert.AreEqual(4, admin.Permissions.Count);
        }

        [TestMethod]
        public void GetPermissionsByRoleId_ReturnsAssignedPermissions()
        {
            List<PermissionBE> perms = _roleMPP.GetPermissionsByRoleId(2);

            Assert.IsNotNull(perms);
            Assert.AreEqual(2, perms.Count);
            Assert.IsTrue(perms.Exists(p => p.Name == "FORM_USER_MGMT"));
            Assert.IsTrue(perms.Exists(p => p.Name == "FORM_COMPLAINTS"));
        }

        [TestMethod]
        public void GetPermissionsByRoleId_IncludesSystemFlag()
        {
            List<PermissionBE> perms = _roleMPP.GetPermissionsByRoleId(1);

            Assert.IsTrue(perms.Exists(p => p.Name == "FORM_USER_MGMT" && p.IsSystem));
            Assert.IsTrue(perms.Exists(p => p.Name == "FORM_COMPLAINTS" && !p.IsSystem));
        }

        [TestMethod]
        public void GetPermissionsByRoleId_NoPermissions_ReturnsEmpty()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    "INSERT INTO Roles (name) VALUES ('Vacio'); SELECT SCOPE_IDENTITY();", connection);
                int newRoleId = System.Convert.ToInt32(cmd.ExecuteScalar());

                List<PermissionBE> perms = _roleMPP.GetPermissionsByRoleId(newRoleId);

                Assert.IsNotNull(perms);
                Assert.AreEqual(0, perms.Count);
            }
        }

        [TestMethod]
        public void GetChildRoleIds_ReturnsDirectChildren()
        {
            List<int> children = _roleMPP.GetChildRoleIds(1);

            CollectionAssert.AreEqual(new List<int> { 2 }, children);
        }

        [TestMethod]
        public void GetChildRoleIds_NoChildren_ReturnsEmpty()
        {
            List<int> children = _roleMPP.GetChildRoleIds(3);

            Assert.AreEqual(0, children.Count);
        }

        [TestMethod]
        public void Save_NewRole_ReturnsNewId()
        {
            var role = new RoleBE { Name = "NuevoRol" };

            int newId = _roleMPP.Save(role);

            Assert.IsTrue(newId > 0);
            Assert.IsNotNull(_roleMPP.FindById(newId));
            Assert.AreEqual("NuevoRol", _roleMPP.FindById(newId).Name);
        }

        [TestMethod]
        public void Save_ExistingRole_UpdatesName()
        {
            RoleBE original = _roleMPP.FindById(2);
            original.Name = "SupervisorActualizado";

            int id = _roleMPP.Save(original);

            Assert.AreEqual(2, id);
            Assert.AreEqual("SupervisorActualizado", _roleMPP.FindById(2).Name);
        }

        [TestMethod]
        public void SavePermissions_ReplacesPermissionSet()
        {
            _roleMPP.SavePermissions(1, new List<int> { 3 });

            List<PermissionBE> perms = _roleMPP.GetPermissionsByRoleId(1);

            Assert.AreEqual(1, perms.Count);
            Assert.AreEqual("FORM_COMPLAINTS", perms[0].Name);
        }

        [TestMethod]
        public void SavePermissions_EmptyList_RemovesAll()
        {
            _roleMPP.SavePermissions(1, new List<int>());

            List<PermissionBE> perms = _roleMPP.GetPermissionsByRoleId(1);

            Assert.AreEqual(0, perms.Count);
        }

        [TestMethod]
        public void Delete_RemovesRoleAndAssociations()
        {
            int newRoleId = _roleMPP.Save(new RoleBE { Name = "RolAEliminar" });
            _roleMPP.SavePermissions(newRoleId, new List<int> { 4 });

            bool result = _roleMPP.Delete(newRoleId);

            Assert.IsTrue(result);
            Assert.IsNull(_roleMPP.FindById(newRoleId));
        }

        [TestMethod]
        public void Delete_NonExistingRole_ReturnsFalse()
        {
            bool result = _roleMPP.Delete(99999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetAllPermissions_ReturnsAllPermissions()
        {
            List<PermissionBE> perms = _roleMPP.GetAllPermissions();

            Assert.IsNotNull(perms);
            Assert.AreEqual(4, perms.Count);
        }
    }
}