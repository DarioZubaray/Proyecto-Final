using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class EnrollmentMPPTests
    {
        #region Propiedades
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private EnrollmentMPP _enrollmentMPP;
        private string _connectionString;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _classroomMPP = new ClassroomMPP(_connectionString);
            _courseMPP = new CourseMPP(_connectionString);
            _enrollmentMPP = new EnrollmentMPP(_connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        private int CreateTestCourse()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula Inscripciones",
                Capacity = 30,
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _classroomMPP.Save(classroom);
            List<ClassroomBE> classrooms = _classroomMPP.FindAll();
            int classroomId = classrooms[classrooms.Count - 1].Id;

            var course = new CourseBE
            {
                Name = "Curso Inscripciones",
                StartDate = new DateTime(2026, 8, 1),
                EndDate = new DateTime(2026, 12, 31),
                ClassroomId = classroomId,
                DayOfWeek = 1,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(10, 0, 0),
                IsActive = true,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
            _courseMPP.Save(course);
            List<CourseBE> courses = _courseMPP.FindAll();
            return courses[courses.Count - 1].Id;
        }
        #endregion

        #region Tests
        [TestMethod]
        public void Enroll_ReturnsTrue()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();

            bool result = _enrollmentMPP.Enroll(courseId, studentId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindByStudentId_ReturnsEnrollments()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            List<EnrollmentBE> result = _enrollmentMPP.FindByStudentId(studentId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
        }

        [TestMethod]
        public void CountStudentsByCourseId_ReturnsCount()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            int count = _enrollmentMPP.CountStudentsByCourseId(courseId);

            Assert.IsTrue(count >= 1);
        }

        [TestMethod]
        public void ExistsActiveEnrollment_True()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            bool result = _enrollmentMPP.ExistsActiveEnrollment(courseId, studentId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsActiveEnrollment_False()
        {
            bool result = _enrollmentMPP.ExistsActiveEnrollment(99999, 99999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Unenroll_DeactivatesEnrollment()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            bool result = _enrollmentMPP.Unenroll(courseId, studentId);

            Assert.IsTrue(result);
            bool exists = _enrollmentMPP.ExistsActiveEnrollment(courseId, studentId);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ExistsScheduleOverlap_Overlap_ReturnsTrue()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            bool result = _enrollmentMPP.ExistsScheduleOverlap(
                studentId, 1, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0), 0);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsScheduleOverlap_NoOverlap_ReturnsFalse()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            bool result = _enrollmentMPP.ExistsScheduleOverlap(
                studentId, 3, new TimeSpan(9, 0, 0), new TimeSpan(11, 0, 0), 0);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExistsScheduleOverlap_ExcludesGivenCourse_ReturnsFalse()
        {
            int courseId = CreateTestCourse();
            int studentId = GetStudentId();
            _enrollmentMPP.Enroll(courseId, studentId);

            bool result = _enrollmentMPP.ExistsScheduleOverlap(
                studentId, 1, new TimeSpan(8, 0, 0), new TimeSpan(12, 0, 0), courseId);

            Assert.IsFalse(result);
        }
        #endregion

        #region Métodos
        private int GetStudentId()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO students (first_name, last_name, document, email, phone, is_active, created_at, last_update)
                      VALUES ('Test', 'Enrollment', 'DNI-INS-1', NULL, NULL, 1, GETDATE(), GETDATE());
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        #endregion
    }
}
