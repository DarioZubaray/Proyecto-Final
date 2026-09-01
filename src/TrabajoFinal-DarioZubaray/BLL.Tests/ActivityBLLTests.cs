using System.Collections.Generic;
using BE.Entities;
using BLL.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MPP;

namespace BLL.Tests
{
    [TestClass]
    public class ActivityBLLTests
    {
        private Mock<IActivityMPP> _mockActivityMPP;
        private ActivityBLL _activityBLL;

        [TestInitialize]
        public void Setup()
        {
            _mockActivityMPP = new Mock<IActivityMPP>();
            _activityBLL = new ActivityBLL(_mockActivityMPP.Object);
        }

        [TestMethod]
        public void LogFormAccess_UsesDecoratorAndSavesLog()
        {
            _activityBLL.LogFormAccess(7, "UserManagementForm", null);

            _mockActivityMPP.Verify(m => m.Save(It.Is<ActivityLogBE>(
                l => l.UserId == 7
                    && l.Action == "FORM_ACCESS"
                    && l.FormName == "UserManagementForm")), Times.Once);
        }

        [TestMethod]
        public void LogLogin_SavesLoginAction()
        {
            _activityBLL.LogLogin(7, "admin");

            _mockActivityMPP.Verify(m => m.Save(It.Is<ActivityLogBE>(
                l => l.UserId == 7 && l.Action == "LOGIN")), Times.Once);
        }

        [TestMethod]
        public void LogLogout_SavesLogoutAction()
        {
            _activityBLL.LogLogout(7, "admin");

            _mockActivityMPP.Verify(m => m.Save(It.Is<ActivityLogBE>(
                l => l.UserId == 7 && l.Action == "LOGOUT")), Times.Once);
        }

        [TestMethod]
        public void Decorator_ExecutesWrappedActivityThenSavesSingleLog()
        {
            IActivity activity = new TestActivity(5, "FORM_ACCESS", "RoleManagementForm");
            IActivity decorated = new ActivityLoggingDecorator(activity, _mockActivityMPP.Object);

            bool result = decorated.Execute();

            Assert.IsTrue(result);
            Assert.AreEqual(5, decorated.UserId);
            Assert.AreEqual("FORM_ACCESS", decorated.Action);
            _mockActivityMPP.Verify(m => m.Save(It.Is<ActivityLogBE>(
                l => l.UserId == 5
                    && l.Action == "FORM_ACCESS"
                    && l.FormName == "RoleManagementForm")), Times.Once);
        }

        [TestMethod]
        public void GetByUserPaginated_DelegatesToMpp()
        {
            var expected = new List<ActivityLogBE>
            {
                new ActivityLogBE(7, "FORM_ACCESS", "LoginForm", null)
            };
            _mockActivityMPP
                .Setup(m => m.GetByUserPaginated(7, 2, 10))
                .Returns(expected);

            List<ActivityLogBE> result = _activityBLL.GetByUserPaginated(7, 2, 10);

            Assert.AreEqual(1, result.Count);
            _mockActivityMPP.Verify(m => m.GetByUserPaginated(7, 2, 10), Times.Once);
        }

        [TestMethod]
        public void CountByUser_DelegatesToMpp()
        {
            _mockActivityMPP.Setup(m => m.CountByUser(7)).Returns(42);

            int count = _activityBLL.CountByUser(7);

            Assert.AreEqual(42, count);
        }

        [TestMethod]
        public void TotalPages_CalculatesCeiling()
        {
            _mockActivityMPP.Setup(m => m.CountByUser(7)).Returns(25);

            Assert.AreEqual(3, _activityBLL.TotalPages(7, 10));
            Assert.AreEqual(1, _activityBLL.TotalPages(7, 50));
        }

        [TestMethod]
        public void TotalPages_AtLeastOnePage()
        {
            _mockActivityMPP.Setup(m => m.CountByUser(7)).Returns(0);

            Assert.AreEqual(1, _activityBLL.TotalPages(7, 10));
        }
    }

    internal class TestActivity : IActivity
    {
        public TestActivity(int userId, string action, string formName)
        {
            UserId = userId;
            Action = action;
            FormName = formName;
        }

        public int UserId { get; private set; }
        public string Action { get; private set; }
        public string FormName { get; private set; }
        public string Description { get; private set; }

        public bool Execute()
        {
            return true;
        }
    }
}
