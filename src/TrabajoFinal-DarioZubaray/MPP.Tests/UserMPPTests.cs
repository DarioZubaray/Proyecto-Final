using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class UserMPPTests
    {
        private UserMPP _userMPP;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _userMPP = new UserMPP();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        [TestMethod]
        public void GetByUserName_ExistingUser_ReturnsUser()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            Assert.IsNotNull(user);
            Assert.AreEqual("testuser", user.UserName);
            Assert.IsTrue(user.IsActive);
        }

        [TestMethod]
        public void GetByUserName_NonExistingUser_ReturnsNull()
        {
            UserBE user = _userMPP.GetByUserName("nonexistent");

            Assert.IsNull(user);
        }

        [TestMethod]
        public void Save_NewUser_ReturnsTrue()
        {
            var newUser = new UserBE
            {
                UserName = "newuser",
                PasswordHash = "hashedpassword",
                IsActive = true,
                RetriesCount = 0,
                LastUpdate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Language = "es",
                RoleId = 1
            };

            bool result = _userMPP.Save(newUser);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindAll_ReturnsListOfUsers()
        {
            List<UserBE> users = _userMPP.FindAll();

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count >= 1);
        }

        [TestMethod]
        public void UpdateRetries_ChangesRetriesCount()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            bool result = _userMPP.UpdateRetries(user.Id, 2);

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.AreEqual(2, updated.RetriesCount);
        }

        [TestMethod]
        public void Deactivate_SetsIsActiveFalse()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            bool result = _userMPP.Deactivate(user.Id);

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.IsFalse(updated.IsActive);
            Assert.AreEqual(3, updated.RetriesCount);
        }
    }
}
