using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class AttendanceMPPTests
    {
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private AttendanceMPP _attendanceMPP;
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
            _attendanceMPP = new AttendanceMPP(_connectionString);
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
                Name = "Aula Asistencia",
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
                Name = "Curso Asistencia",
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

        private int CreateTestStudent()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('test_attendance_student', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'System', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        public void RegisterAttendanceBulk_ReturnsTrue()
        {
            int courseId = CreateTestCourse();
            int studentId = CreateTestStudent();
            DateTime date = new DateTime(2026, 9, 1);
            var records = new List<AttendanceBE>
            {
                new AttendanceBE { StudentId = studentId, IsPresent = true }
            };

            bool result = _attendanceMPP.RegisterAttendanceBulk(courseId, date, records);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindByCourseIdAndDate_ReturnsRecords()
        {
            int courseId = CreateTestCourse();
            int studentId = CreateTestStudent();
            DateTime date = new DateTime(2026, 9, 1);
            _attendanceMPP.RegisterAttendanceBulk(courseId, date, new List<AttendanceBE>
            {
                new AttendanceBE { StudentId = studentId, IsPresent = true }
            });

            List<AttendanceBE> result = _attendanceMPP.FindByCourseIdAndDate(courseId, date);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
        }

        [TestMethod]
        public void FindByCourseIdAndDate_NoRecords_ReturnsEmptyList()
        {
            List<AttendanceBE> result = _attendanceMPP.FindByCourseIdAndDate(99999, DateTime.Now);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void IsTeacherOfCourse_True()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher();

            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    $@"INSERT INTO course_teachers (course_id, teacher_id, is_active, created_at)
                      VALUES ({courseId}, {teacherId}, 1, GETDATE())", connection);
                cmd.ExecuteNonQuery();
            }

            bool result = _attendanceMPP.IsTeacherOfCourse(courseId, teacherId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsTeacherOfCourse_False()
        {
            bool result = _attendanceMPP.IsTeacherOfCourse(99999, 99999);

            Assert.IsFalse(result);
        }

        private int CreateTestTeacher()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('test_teacher', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'System', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
