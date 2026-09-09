using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class ActivityMPPTests
    {
        private ActivityMPP _activityMPP;
        private int _testUserId;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _activityMPP = new ActivityMPP(_connectionString);

            UserBE user = new UserMPP(_connectionString).GetByUserName("testuser");
            _testUserId = user != null ? user.Id : 1;
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        [TestMethod]
        public void Save_InsertsActivityLog()
        {
            var log = new ActivityLogBE(_testUserId, "FORM_ACCESS", "LoginForm", null);

            bool result = _activityMPP.Save(log);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Save_ThenPaginatedRead_ReturnsOwnRecord()
        {
            _activityMPP.Save(new ActivityLogBE(_testUserId, "FORM_ACCESS", "UserManagementForm", null));

            List<ActivityLogBE> logs = _activityMPP.GetByUserPaginated(_testUserId, 1, 10);

            Assert.IsNotNull(logs);
            Assert.IsTrue(logs.Exists(l => l.FormName == "UserManagementForm"));
        }

        [TestMethod]
        public void CountByUser_CountsOnlyMatchingUser()
        {
            _activityMPP.Save(new ActivityLogBE(_testUserId, "LOGIN", null, null));
            _activityMPP.Save(new ActivityLogBE(_testUserId, "LOGOUT", null, null));

            int count = _activityMPP.CountByUser(_testUserId);

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void Pagination_OffsetAndPageSize_Respected()
        {
            for (int i = 0; i < 5; i++)
            {
                _activityMPP.Save(new ActivityLogBE(_testUserId, "FORM_ACCESS", "Form" + i, null));
            }

            List<ActivityLogBE> page1 = _activityMPP.GetByUserPaginated(_testUserId, 1, 3);
            List<ActivityLogBE> page2 = _activityMPP.GetByUserPaginated(_testUserId, 2, 3);

            Assert.AreEqual(3, page1.Count);
            Assert.AreEqual(2, page2.Count);
        }
    }
}