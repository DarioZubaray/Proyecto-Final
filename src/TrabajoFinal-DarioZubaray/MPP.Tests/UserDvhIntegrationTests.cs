using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using BLL.Services;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class UserDvhIntegrationTests
    {
        private UserMPP _userMPP;
        private UserBLL _userBLL;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _userMPP = new UserMPP(_connectionString);
            _userBLL = new UserBLL(_userMPP);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        [TestMethod]
        public void Save_NewUser_RegeneratesDvh()
        {
            var newUser = new UserBE
            {
                UserName = "dvhuser",
                PasswordHash = "hashedpassword",
                IsActive = true,
                RetriesCount = 0,
                LastUpdate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Language = "es",
                Theme = "System",
                RoleId = 2
            };

            Assert.IsTrue(_userBLL.Save(newUser));

            UserBE persisted = _userMPP.GetByUserName("dvhuser");

            Assert.IsNotNull(persisted);
            Assert.IsFalse(string.IsNullOrEmpty(persisted.Dvh));
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void Save_ExistingUser_RegeneratesDvh()
        {
            UserBE user = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(user);

            user.UserName = "dvhrenamed";
            Assert.IsTrue(_userBLL.Save(user));

            UserBE persisted = _userMPP.GetByUserName("dvhrenamed");

            Assert.IsNotNull(persisted);
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void UpdatePartialFields_RegenerateDvh()
        {
            UserBE user = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(user);

            Assert.IsTrue(_userBLL.UpdateLanguage(user.Id, "en"));
            Assert.IsTrue(_userBLL.UpdateTheme(user.Id, "Dark"));

            UserBE persisted = _userMPP.GetByUserName("testuser");

            Assert.IsNotNull(persisted.Dvh);
            Assert.AreEqual("en", persisted.Language);
            Assert.AreEqual("Dark", persisted.Theme);
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void Delete_RegeneratesDvh()
        {
            UserBE user = _userMPP.GetByUserName("seconduser");
            Assert.IsNotNull(user);

            Assert.IsTrue(_userBLL.Delete(user));

            UserBE persisted = _userMPP.GetByUserName("seconduser");

            Assert.IsFalse(persisted.IsActive);
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void ChangePassword_RegeneratesDvh()
        {
            var newUser = new UserBE
            {
                UserName = "changepass",
                PasswordHash = EncryptionBLL.HashPassword("correctpassword"),
                IsActive = true,
                RetriesCount = 0,
                LastUpdate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Language = "es",
                Theme = "System",
                RoleId = 2
            };

            Assert.IsTrue(_userBLL.Save(newUser));

            Assert.IsTrue(_userBLL.ChangePassword(newUser.Id, "correctpassword", "newpassword123"));

            UserBE persisted = _userMPP.GetByUserName("changepass");

            Assert.IsNotNull(persisted.Dvh);
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void AlteredRowOutOfBand_StoredDvhDoesNotMatch()
        {
            UserBE user = _userMPP.GetByUserName("testuser");
            Assert.IsNotNull(user);
            Assert.IsTrue(_userBLL.Save(user));

            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    "UPDATE users SET user_name = 'alterado' WHERE user_name = 'testuser'",
                    connection);
                cmd.ExecuteNonQuery();
            }

            UserBE altered = _userMPP.GetByUserName("alterado");

            Assert.IsNotNull(altered);
            Assert.AreNotEqual(DVHHelperBLL.Compute(altered.GetDvhValues()), altered.Dvh);
        }

        [TestMethod]
        public void Dvh_BackfillExpression_MatchesApplicationHash()
        {
            DateTime[] stamps = new DateTime[]
            {
                new DateTime(2026, 10, 9, 13, 45, 59, 998),
                new DateTime(2026, 10, 9, 13, 45, 59, 999),
                new DateTime(2026, 1, 2, 8, 30, 0, 123),
                new DateTime(2026, 6, 15, 23, 59, 59, 990),
                new DateTime(2026, 3, 3, 0, 0, 0, 10)
            };

            for (int i = 0; i < stamps.Length; i++)
            {
                var newUser = new UserBE
                {
                    UserName = "dvhparity" + i,
                    PasswordHash = "hashedpassword" + i,
                    IsActive = i % 2 == 0,
                    RetriesCount = i,
                    LastUpdate = stamps[i],
                    CreatedAt = stamps[i],
                    Language = "es",
                    Theme = "System",
                    RoleId = 2
                };

                Assert.IsTrue(_userBLL.Save(newUser));
            }

            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"UPDATE u
                      SET u.[dvh] = LOWER(CONVERT(VARCHAR(64),
                          HASHBYTES('SHA2_256',
                              CONCAT(
                                  CONVERT(NVARCHAR(MAX), u.[id]),          ',',
                                  u.[user_name],                           ',',
                                  u.[password_hash],                       ',',
                                  CASE WHEN u.[is_active] = 1 THEN '1' ELSE '0' END, ',',
                                  CONVERT(NVARCHAR(MAX), u.[retries_count]), ',',
                                  CONVERT(NVARCHAR(30), u.[last_update], 126), ',',
                                  CONVERT(NVARCHAR(30), u.[created_at], 126),  ',',
                                  CASE WHEN u.[role_id] IS NULL THEN '' ELSE CONVERT(NVARCHAR(MAX), u.[role_id]) END, ',',
                                  u.[language],                            ',',
                                  u.[theme]
                              )
                          ), 2))
                      FROM [dbo].[users] u
                      WHERE u.[user_name] LIKE 'dvhparity%'",
                    connection);
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < stamps.Length; i++)
            {
                UserBE persisted = _userMPP.GetByUserName("dvhparity" + i);

                Assert.IsNotNull(persisted);
                Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh,
                    "Paridad DVH fallida para dvhparity" + i);
            }
        }
    }
}
