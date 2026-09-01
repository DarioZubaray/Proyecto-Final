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
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _userMPP = new UserMPP(_connectionString);
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
        public void GetByUserName_Idempotent_IsLatest()
        {
            UserBE first = _userMPP.GetByUserName("testuser");
            UserBE second = _userMPP.GetByUserName("testuser");

            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
            Assert.AreEqual(first.Id, second.Id);
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
                Theme = "System",
                RoleId = 2
            };

            bool result = _userMPP.Save(newUser);

            Assert.IsTrue(result);
            Assert.IsNotNull(_userMPP.GetByUserName("newuser"));
        }

        [TestMethod]
        public void Save_ExistingUser_UpdatesRecord()
        {
            UserBE user = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(user);

            user.UserName = "renameduser";
            bool result = _userMPP.Save(user);

            Assert.IsTrue(result);
            Assert.IsNull(_userMPP.GetByUserName("testuser"));
            Assert.IsNotNull(_userMPP.GetByUserName("renameduser"));
        }

        [TestMethod]
        public void UpdateLastUpdate_ChangesTimestamp()
        {
            UserBE user = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(user);

            DateTime original = user.LastUpdate;
            DateTime newStamp = DateTime.Now;
            bool result = _userMPP.UpdateLastUpdate(user.Id, newStamp);

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(updated);
            Assert.IsTrue(updated.LastUpdate > original);
            Assert.IsTrue((updated.LastUpdate - newStamp).Duration() < TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void FindAll_ReturnsListOfUsers()
        {
            List<UserBE> users = _userMPP.FindAll();

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Count >= 3);
        }

        [TestMethod]
        public void FindById_ExistingUser_ReturnsUser()
        {
            UserBE seeded = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(seeded);

            UserBE found = _userMPP.FindById(new UserBE { Id = seeded.Id });

            Assert.IsNotNull(found);
            Assert.AreEqual(seeded.Id, found.Id);
            Assert.AreEqual("testuser", found.UserName);
        }

        [TestMethod]
        public void FindById_NonExistingUser_ReturnsNull()
        {
            UserBE found = _userMPP.FindById(new UserBE { Id = 99999 });

            Assert.IsNull(found);
        }

        [TestMethod]
        public void FindByUserName_FiltersPartialMatch()
        {
            List<UserBE> matches = _userMPP.FindByUserName("user");

            Assert.IsNotNull(matches);
            Assert.IsTrue(matches.Exists(u => u.UserName == "testuser"));
            Assert.IsTrue(matches.Exists(u => u.UserName == "seconduser"));
            Assert.IsTrue(matches.Exists(u => u.UserName == "thirduser"));
        }

        [TestMethod]
        public void Delete_DeactivatesUser()
        {
            UserBE user = _userMPP.GetByUserName("seconduser");
            Assert.IsNotNull(user);

            bool result = _userMPP.Delete(user);

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("seconduser");
            Assert.IsNotNull(updated);
            Assert.IsFalse(updated.IsActive);
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

        [TestMethod]
        public void CountByRoleId_CountsMatchingUsers()
        {
            int count = _userMPP.CountByRoleId(1);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void CountByRoleId_NoUsers_ReturnsZero()
        {
            int count = _userMPP.CountByRoleId(99);

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void TestConnection_ReturnsTrue()
        {
            Assert.IsTrue(_userMPP.TestConnection());
        }

        [TestMethod]
        public void UpdateLanguage_ChangesLanguage()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            bool result = _userMPP.UpdateLanguage(user.Id, "pt-BR");

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.AreEqual("pt-BR", updated.Language);
        }

        [TestMethod]
        public void UpdateTheme_ChangesTheme()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            bool result = _userMPP.UpdateTheme(user.Id, "Dark");

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.AreEqual("Dark", updated.Theme);
        }

        [TestMethod]
        public void UpdatePassword_ChangesPasswordHash()
        {
            UserBE user = _userMPP.GetByUserName("testuser");

            bool result = _userMPP.UpdatePassword(user.Id, "newhashedpassword");

            Assert.IsTrue(result);

            UserBE updated = _userMPP.GetByUserName("testuser");
            Assert.AreEqual("newhashedpassword", updated.PasswordHash);
        }

        [TestMethod]
        public void MapUser_NullRoleId_ReturnsZero_AndDefaultThemeApplies()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO Users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('nullexisting', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'CustomTheme', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                cmd.ExecuteScalar();
            }

            UserBE user = _userMPP.GetByUserName("nullexisting");

            Assert.IsNotNull(user);
            Assert.AreEqual("CustomTheme", user.Theme);
            Assert.AreEqual(0, user.RoleId);
        }
    }
}