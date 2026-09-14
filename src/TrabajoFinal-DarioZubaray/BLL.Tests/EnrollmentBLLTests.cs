using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Interfaces;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class EnrollmentBLLTests
    {
        #region Propiedades
        private Mock<IEnrollmentMPP> _mockEnrollmentMPP;
        private Mock<ICourseBLL> _mockCourseBLL;
        private EnrollmentBLL _enrollmentBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockEnrollmentMPP = new Mock<IEnrollmentMPP>();
            _mockCourseBLL = new Mock<ICourseBLL>();
            _enrollmentBLL = new EnrollmentBLL(_mockEnrollmentMPP.Object, _mockCourseBLL.Object);
        }
        #endregion

        #region Tests - Enroll
        [TestMethod]
        public void Enroll_Success()
        {
            int courseId = 1;
            int studentId = 20;

            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(courseId, studentId)).Returns(false);
            _mockCourseBLL.Setup(m => m.FindById(courseId)).Returns(new CourseBE
            {
                Id = courseId,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            });
            _mockEnrollmentMPP.Setup(m => m.FindByStudentId(studentId)).Returns(new List<EnrollmentBE>());
            _mockEnrollmentMPP.Setup(m => m.Enroll(courseId, studentId)).Returns(true);

            bool result = _enrollmentBLL.Enroll(courseId, studentId);

            Assert.IsTrue(result);
            _mockEnrollmentMPP.Verify(m => m.Enroll(courseId, studentId), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enroll_AlreadyEnrolled_ThrowsException()
        {
            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(1, 20)).Returns(true);

            _enrollmentBLL.Enroll(1, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enroll_CourseNotFound_ThrowsException()
        {
            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(999, 20)).Returns(false);
            _mockCourseBLL.Setup(m => m.FindById(999)).Returns((CourseBE)null);

            _enrollmentBLL.Enroll(999, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enroll_CourseNoSchedule_ThrowsException()
        {
            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(1, 20)).Returns(false);
            _mockCourseBLL.Setup(m => m.FindById(1)).Returns(new CourseBE
            {
                Id = 1,
                DayOfWeek = null,
                StartTime = null,
                EndTime = null
            });

            _enrollmentBLL.Enroll(1, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Enroll_ScheduleOverlap_ThrowsException()
        {
            int courseId = 1;
            int studentId = 20;

            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(courseId, studentId)).Returns(false);
            _mockCourseBLL.Setup(m => m.FindById(courseId)).Returns(new CourseBE
            {
                Id = courseId,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            });

            var existingEnrollments = new List<EnrollmentBE>
            {
                new EnrollmentBE
                {
                    CourseId = 2,
                    DayOfWeek = 1,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    CourseName = "Frances"
                }
            };
            _mockEnrollmentMPP.Setup(m => m.FindByStudentId(studentId)).Returns(existingEnrollments);
            _mockEnrollmentMPP.Setup(m => m.ExistsScheduleOverlap(studentId, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), courseId)).Returns(true);

            _enrollmentBLL.Enroll(courseId, studentId);
        }
        #endregion

        #region Tests - Unenroll
        [TestMethod]
        public void Unenroll_DelegatesToMPP()
        {
            _mockEnrollmentMPP.Setup(m => m.Unenroll(1, 20)).Returns(true);

            bool result = _enrollmentBLL.Unenroll(1, 20);

            Assert.IsTrue(result);
            _mockEnrollmentMPP.Verify(m => m.Unenroll(1, 20), Times.Once);
        }
        #endregion

        #region Tests - Find
        [TestMethod]
        public void FindById_DelegatesToMPP()
        {
            var expected = new EnrollmentBE { Id = 1, CourseId = 1, StudentId = 20 };
            _mockEnrollmentMPP.Setup(m => m.FindById(1)).Returns(expected);

            EnrollmentBE result = _enrollmentBLL.FindById(1);

            Assert.AreEqual(expected, result);
            _mockEnrollmentMPP.Verify(m => m.FindById(1), Times.Once);
        }

        [TestMethod]
        public void FindByStudentId_DelegatesToMPP()
        {
            var expected = new List<EnrollmentBE>
            {
                new EnrollmentBE { Id = 1, CourseId = 1, StudentId = 20 }
            };
            _mockEnrollmentMPP.Setup(m => m.FindByStudentId(20)).Returns(expected);

            List<EnrollmentBE> result = _enrollmentBLL.FindByStudentId(20);

            CollectionAssert.AreEqual(expected, result);
            _mockEnrollmentMPP.Verify(m => m.FindByStudentId(20), Times.Once);
        }

        [TestMethod]
        public void FindAll_DelegatesToMPP()
        {
            var expected = new List<EnrollmentBE>
            {
                new EnrollmentBE { Id = 1 },
                new EnrollmentBE { Id = 2 }
            };
            _mockEnrollmentMPP.Setup(m => m.FindAll()).Returns(expected);

            List<EnrollmentBE> result = _enrollmentBLL.FindAll();

            CollectionAssert.AreEqual(expected, result);
            _mockEnrollmentMPP.Verify(m => m.FindAll(), Times.Once);
        }
        #endregion

        #region Tests - Count & Exists
        [TestMethod]
        public void CountStudentsByCourseId_DelegatesToMPP()
        {
            _mockEnrollmentMPP.Setup(m => m.CountStudentsByCourseId(1)).Returns(15);

            int result = _enrollmentBLL.CountStudentsByCourseId(1);

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void ExistsActiveEnrollment_DelegatesToMPP()
        {
            _mockEnrollmentMPP.Setup(m => m.ExistsActiveEnrollment(1, 20)).Returns(true);

            bool result = _enrollmentBLL.ExistsActiveEnrollment(1, 20);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsScheduleOverlap_DelegatesToMPP()
        {
            _mockEnrollmentMPP.Setup(m => m.ExistsScheduleOverlap(20, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 1)).Returns(false);

            bool result = _enrollmentBLL.ExistsScheduleOverlap(20, 1, new TimeSpan(8, 0, 0), new TimeSpan(10, 0, 0), 1);

            Assert.IsFalse(result);
        }
        #endregion
    }
}
