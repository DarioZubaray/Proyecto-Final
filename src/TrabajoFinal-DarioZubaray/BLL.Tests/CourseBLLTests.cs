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
    public class CourseBLLTests
    {
        #region Propiedades
        private Mock<ICourseMPP> _mockCourseMPP;
        private CourseBLL _courseBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockCourseMPP = new Mock<ICourseMPP>();
            _courseBLL = new CourseBLL(_mockCourseMPP.Object);
        }
        #endregion

        #region Tests - Save
        [TestMethod]
        public void Save_NoOverlap_DelegatesToMPP()
        {
            var course = new CourseBE
            {
                Id = 0,
                ClassroomId = 1,
                StartDate = new DateTime(2026, 8, 1),
                EndDate = new DateTime(2026, 12, 31),
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };
            _mockCourseMPP.Setup(m => m.ExistsClassroomOverlap(1, course.StartDate, course.EndDate, 0, (int?)1, (TimeSpan?)course.StartTime, (TimeSpan?)course.EndTime)).Returns(false);
            _mockCourseMPP.Setup(m => m.Save(course)).Returns(true);

            bool result = _courseBLL.Save(course);

            Assert.IsTrue(result);
            _mockCourseMPP.Verify(m => m.Save(course), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Save_WithClassroomOverlap_ThrowsException()
        {
            var course = new CourseBE
            {
                Id = 0,
                ClassroomId = 1,
                StartDate = new DateTime(2026, 8, 1),
                EndDate = new DateTime(2026, 12, 31),
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };
            _mockCourseMPP.Setup(m => m.ExistsClassroomOverlap(1, course.StartDate, course.EndDate, 0, (int?)1, (TimeSpan?)course.StartTime, (TimeSpan?)course.EndTime)).Returns(true);

            _courseBLL.Save(course);
        }

        [TestMethod]
        public void Save_DelegatesToMPP()
        {
            var course = new CourseBE { Id = 5 };
            _mockCourseMPP.Setup(m => m.ExistsClassroomOverlap(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>())).Returns(false);
            _mockCourseMPP.Setup(m => m.Save(course)).Returns(true);

            bool result = _courseBLL.Save(course);

            Assert.IsTrue(result);
        }
        #endregion

        #region Tests - Delete
        [TestMethod]
        public void Delete_DelegatesToMPP()
        {
            var course = new CourseBE { Id = 3 };
            _mockCourseMPP.Setup(m => m.Delete(course)).Returns(true);

            bool result = _courseBLL.Delete(course);

            Assert.IsTrue(result);
            _mockCourseMPP.Verify(m => m.Delete(course), Times.Once);
        }
        #endregion

        #region Tests - Find
        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var expected = new CourseBE { Id = 1, Name = "Ingles Basico" };
            _mockCourseMPP.Setup(m => m.FindById(1)).Returns(expected);

            CourseBE result = _courseBLL.FindById(1);

            Assert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindById(1), Times.Once);
        }

        [TestMethod]
        public void FindById_NonExisting_ReturnsNull()
        {
            _mockCourseMPP.Setup(m => m.FindById(999)).Returns((CourseBE)null);

            CourseBE result = _courseBLL.FindById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expected = new List<CourseBE>
            {
                new CourseBE { Id = 1, Name = "Ingles Basico" },
                new CourseBE { Id = 2, Name = "Frances Inicial" }
            };
            _mockCourseMPP.Setup(m => m.FindAll()).Returns(expected);

            List<CourseBE> result = _courseBLL.FindAll();

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindAll(), Times.Once);
        }

        [TestMethod]
        public void FindAllIncludingInactive_DelegatesToMPP()
        {
            var expected = new List<CourseBE>
            {
                new CourseBE { Id = 1, Name = "Ingles Basico", IsActive = true },
                new CourseBE { Id = 2, Name = "Frances Inicial", IsActive = false }
            };
            _mockCourseMPP.Setup(m => m.FindAllIncludingInactive()).Returns(expected);

            List<CourseBE> result = _courseBLL.FindAllIncludingInactive();

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindAllIncludingInactive(), Times.Once);
        }

        [TestMethod]
        public void FindByName_DelegatesToMPP()
        {
            var expected = new List<CourseBE>
            {
                new CourseBE { Id = 1, Name = "Ingles Basico" }
            };
            _mockCourseMPP.Setup(m => m.FindByName("Ingles")).Returns(expected);

            List<CourseBE> result = _courseBLL.FindByName("Ingles");

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindByName("Ingles"), Times.Once);
        }

        [TestMethod]
        public void FindByTeacherId_DelegatesToMPP()
        {
            var expected = new List<CourseBE>
            {
                new CourseBE { Id = 1, Name = "Ingles Basico" }
            };
            _mockCourseMPP.Setup(m => m.FindByTeacherId(10)).Returns(expected);

            List<CourseBE> result = _courseBLL.FindByTeacherId(10);

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindByTeacherId(10), Times.Once);
        }

        [TestMethod]
        public void FindByStudentId_DelegatesToMPP()
        {
            var expected = new List<CourseBE>
            {
                new CourseBE { Id = 1, Name = "Ingles Basico" }
            };
            _mockCourseMPP.Setup(m => m.FindByStudentId(20)).Returns(expected);

            List<CourseBE> result = _courseBLL.FindByStudentId(20);

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.FindByStudentId(20), Times.Once);
        }
        #endregion

        #region Tests - Teachers
        [TestMethod]
        public void SaveTeachers_DelegatesToMPP()
        {
            _mockCourseMPP.Setup(m => m.SaveTeachers(1, new List<int> { 10, 11 })).Returns(true);

            bool result = _courseBLL.SaveTeachers(1, new List<int> { 10, 11 });

            Assert.IsTrue(result);
            _mockCourseMPP.Verify(m => m.SaveTeachers(1, new List<int> { 10, 11 }), Times.Once);
        }

        [TestMethod]
        public void GetTeachersByCourseId_DelegatesToMPP()
        {
            var expected = new List<UserBE>
            {
                new UserBE { Id = 10, UserName = "prof_garcia" }
            };
            _mockCourseMPP.Setup(m => m.GetTeachersByCourseId(1)).Returns(expected);

            List<UserBE> result = _courseBLL.GetTeachersByCourseId(1);

            CollectionAssert.AreEqual(expected, result);
            _mockCourseMPP.Verify(m => m.GetTeachersByCourseId(1), Times.Once);
        }
        #endregion

        #region Tests - Overlap Validation
        [TestMethod]
        public void ValidateTeacherOverlap_NoOverlap_ReturnsFalse()
        {
            var teacherIds = new List<int> { 10, 11 };
            _mockCourseMPP.Setup(m => m.ExistsTeacherOverlap(10, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 0)).Returns(false);
            _mockCourseMPP.Setup(m => m.ExistsTeacherOverlap(11, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 0)).Returns(false);

            bool result = _courseBLL.ValidateTeacherOverlap(0, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), teacherIds);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTeacherOverlap_WithOverlap_ReturnsTrue()
        {
            var teacherIds = new List<int> { 10, 11 };
            _mockCourseMPP.Setup(m => m.ExistsTeacherOverlap(10, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 0)).Returns(false);
            _mockCourseMPP.Setup(m => m.ExistsTeacherOverlap(11, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 0)).Returns(true);

            bool result = _courseBLL.ValidateTeacherOverlap(0, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), teacherIds);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateTeacherOverlap_NullSchedule_ReturnsFalse()
        {
            bool result = _courseBLL.ValidateTeacherOverlap(0, null, null, null, new List<int> { 10 });

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExistsClassroomOverlap_DelegatesToMPP()
        {
            _mockCourseMPP.Setup(m => m.ExistsClassroomOverlap(1, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 0, (int?)1, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>())).Returns(true);

            bool result = _courseBLL.ExistsClassroomOverlap(1, DateTime.Now, DateTime.Now, 0, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0));

            Assert.IsTrue(result);
        }
        #endregion
    }
}
