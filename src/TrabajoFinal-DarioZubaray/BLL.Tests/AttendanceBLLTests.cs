using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class AttendanceBLLTests
    {
        #region Propiedades
        private Mock<IAttendanceMPP> _mockAttendanceMPP;
        private AttendanceBLL _attendanceBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockAttendanceMPP = new Mock<IAttendanceMPP>();
            _attendanceBLL = new AttendanceBLL(_mockAttendanceMPP.Object);
        }
        #endregion

        #region Tests - RegisterAttendanceBulk
        [TestMethod]
        public void RegisterAttendanceBulk_DelegatesToMPP()
        {
            int courseId = 1;
            DateTime date = new DateTime(2026, 9, 1);
            var records = new List<AttendanceBE>
            {
                new AttendanceBE { StudentId = 20, IsPresent = true },
                new AttendanceBE { StudentId = 21, IsPresent = false }
            };
            _mockAttendanceMPP.Setup(m => m.RegisterAttendanceBulk(courseId, date, records)).Returns(true);

            bool result = _attendanceBLL.RegisterAttendanceBulk(courseId, date, records);

            Assert.IsTrue(result);
            _mockAttendanceMPP.Verify(m => m.RegisterAttendanceBulk(courseId, date, records), Times.Once);
        }

        [TestMethod]
        public void RegisterAttendanceBulk_EmptyRecords_DelegatesToMPP()
        {
            int courseId = 1;
            DateTime date = new DateTime(2026, 9, 1);
            var records = new List<AttendanceBE>();
            _mockAttendanceMPP.Setup(m => m.RegisterAttendanceBulk(courseId, date, records)).Returns(true);

            bool result = _attendanceBLL.RegisterAttendanceBulk(courseId, date, records);

            Assert.IsTrue(result);
        }
        #endregion

        #region Tests - FindByCourseIdAndDate
        [TestMethod]
        public void FindByCourseIdAndDate_DelegatesToMPP()
        {
            int courseId = 1;
            DateTime date = new DateTime(2026, 9, 1);
            var expected = new List<AttendanceBE>
            {
                new AttendanceBE { Id = 1, CourseId = 1, StudentId = 20, IsPresent = true, StudentName = "alumno_perez" },
                new AttendanceBE { Id = 2, CourseId = 1, StudentId = 21, IsPresent = false, StudentName = "alumno_lopez" }
            };
            _mockAttendanceMPP.Setup(m => m.FindByCourseIdAndDate(courseId, date)).Returns(expected);

            List<AttendanceBE> result = _attendanceBLL.FindByCourseIdAndDate(courseId, date);

            Assert.AreEqual(2, result.Count);
            _mockAttendanceMPP.Verify(m => m.FindByCourseIdAndDate(courseId, date), Times.Once);
        }

        [TestMethod]
        public void FindByCourseIdAndDate_NoRecords_ReturnsEmptyList()
        {
            int courseId = 1;
            DateTime date = new DateTime(2026, 9, 1);
            _mockAttendanceMPP.Setup(m => m.FindByCourseIdAndDate(courseId, date)).Returns(new List<AttendanceBE>());

            List<AttendanceBE> result = _attendanceBLL.FindByCourseIdAndDate(courseId, date);

            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region Tests - IsTeacherOfCourse
        [TestMethod]
        public void IsTeacherOfCourse_True()
        {
            _mockAttendanceMPP.Setup(m => m.IsTeacherOfCourse(1, 10)).Returns(true);

            bool result = _attendanceBLL.IsTeacherOfCourse(1, 10);

            Assert.IsTrue(result);
            _mockAttendanceMPP.Verify(m => m.IsTeacherOfCourse(1, 10), Times.Once);
        }

        [TestMethod]
        public void IsTeacherOfCourse_False()
        {
            _mockAttendanceMPP.Setup(m => m.IsTeacherOfCourse(1, 99)).Returns(false);

            bool result = _attendanceBLL.IsTeacherOfCourse(1, 99);

            Assert.IsFalse(result);
        }
        #endregion
    }
}
