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
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private EnrollmentMPP _enrollmentMPP;
        private string _connectionString;

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

        private int GetStudentId()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('test_student', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'System', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
