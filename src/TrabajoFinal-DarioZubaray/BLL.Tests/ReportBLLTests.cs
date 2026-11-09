using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class ReportBLLTests
    {
        #region Propiedades
        private Mock<IReportMPP> _mockReportMPP;
        private ReportBLL _reportBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockReportMPP = new Mock<IReportMPP>();
            _reportBLL = new ReportBLL(_mockReportMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void GetCoursePerformance_DelegatesToMPP()
        {
            var expected = new List<StudentPerformanceBE>
            {
                new StudentPerformanceBE { StudentId = 1, StudentName = "Juan Pérez", AttendancePercent = 80, GradesCount = 2, Average = 8.5m }
            };
            _mockReportMPP.Setup(m => m.GetCoursePerformance(10)).Returns(expected);

            List<StudentPerformanceBE> result = _reportBLL.GetCoursePerformance(10);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(8.5m, result[0].Average);
            _mockReportMPP.Verify(m => m.GetCoursePerformance(10), Times.Once);
        }

        [TestMethod]
        public void GetAttendanceReport_DelegatesToMPP()
        {
            var expected = new List<AttendanceReportLineBE>
            {
                new AttendanceReportLineBE { StudentId = 1, StudentName = "Juan Pérez", TotalPlans = 5, PresentPlans = 4, AttendancePercent = 80 }
            };
            _mockReportMPP.Setup(m => m.GetAttendanceReport(10)).Returns(expected);

            List<AttendanceReportLineBE> result = _reportBLL.GetAttendanceReport(10);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(80, result[0].AttendancePercent);
            _mockReportMPP.Verify(m => m.GetAttendanceReport(10), Times.Once);
        }

        [TestMethod]
        public void GetGradesReport_DelegatesToMPP()
        {
            var expected = new List<GradeReportLineBE>
            {
                new GradeReportLineBE { StudentId = 1, StudentName = "Juan Pérez", EvaluationsCount = 3, GradesCount = 2, Average = 7.25m }
            };
            _mockReportMPP.Setup(m => m.GetGradesReport(10)).Returns(expected);

            List<GradeReportLineBE> result = _reportBLL.GetGradesReport(10);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(7.25m, result[0].Average);
            _mockReportMPP.Verify(m => m.GetGradesReport(10), Times.Once);
        }
        #endregion
    }
}
