using System.Collections.Generic;
using BE.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class ReportMPPTests
    {
        private ReportMPP _reportMPP;
        private string _connectionString;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _reportMPP = new ReportMPP(_connectionString);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        #region Métodos auxiliares
        private void Execute(string sql)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private int CreateCourseWithAttendanceAndGrades()
        {
            Execute(@"INSERT INTO classrooms (name, capacity, is_active, created_at, last_update)
                      VALUES ('Aula Reportes', 30, 1, GETDATE(), GETDATE());
                      INSERT INTO courses (name, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update)
                      VALUES ('Curso Reportes', '2026-08-01', '2026-12-31', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE());
                      INSERT INTO course_students (course_id, student_id, is_active, created_at) VALUES (1, 1, 1, GETDATE()), (1, 2, 1, GETDATE());
                      INSERT INTO attendance (course_id, student_id, date, is_present, created_at) VALUES
                        (1, 1, '2026-08-05', 1, GETDATE()),
                        (1, 1, '2026-08-12', 1, GETDATE()),
                        (1, 1, '2026-08-19', 0, GETDATE()),
                        (1, 2, '2026-08-05', 1, GETDATE()),
                        (1, 2, '2026-08-12', 0, GETDATE()),
                        (1, 2, '2026-08-19', 0, GETDATE());
                      INSERT INTO evaluations (course_id, name, date, created_by, created_at, last_update) VALUES
                        (1, 'Parcial 1', '2026-09-15', 1, GETDATE(), GETDATE()),
                        (1, 'Parcial 2', '2026-10-20', 1, GETDATE(), GETDATE());
                      INSERT INTO grades (evaluation_id, student_id, grade) VALUES (1, 1, 8), (2, 1, 10);");

            return 1;
        }

        private int CreateCourseWithStudentOnly()
        {
            Execute(@"INSERT INTO classrooms (name, capacity, is_active, created_at, last_update)
                      VALUES ('Aula Vacia', 30, 1, GETDATE(), GETDATE());
                      INSERT INTO courses (name, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update)
                      VALUES ('Curso Sin Datos', '2026-08-01', '2026-12-31', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE());
                      INSERT INTO course_students (course_id, student_id, is_active, created_at) VALUES (1, 1, 1, GETDATE());");

            return 1;
        }
        #endregion

        [TestMethod]
        public void GetCoursePerformance_ReturnsRowPerEnrolledStudent()
        {
            int courseId = CreateCourseWithAttendanceAndGrades();

            List<StudentPerformanceBE> result = _reportMPP.GetCoursePerformance(courseId);

            Assert.AreEqual(2, result.Count);

            StudentPerformanceBE juan = result.Find(l => l.StudentId == 1);
            Assert.AreEqual(67, juan.AttendancePercent);
            Assert.AreEqual(2, juan.GradesCount);
            Assert.AreEqual(9m, juan.Average);

            StudentPerformanceBE maria = result.Find(l => l.StudentId == 2);
            Assert.AreEqual(33, maria.AttendancePercent);
            Assert.AreEqual(0, maria.GradesCount);
            Assert.IsNull(maria.Average);
        }

        [TestMethod]
        public void GetCoursePerformance_WithoutAttendanceOrGrades_ReturnsNulls()
        {
            int courseId = CreateCourseWithStudentOnly();

            List<StudentPerformanceBE> result = _reportMPP.GetCoursePerformance(courseId);

            Assert.AreEqual(1, result.Count);
            Assert.IsNull(result[0].AttendancePercent);
            Assert.AreEqual(0, result[0].GradesCount);
            Assert.IsNull(result[0].Average);
        }

        [TestMethod]
        public void GetCoursePerformance_NoEnrollments_ReturnsEmptyList()
        {
            List<StudentPerformanceBE> result = _reportMPP.GetCoursePerformance(99999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetAttendanceReport_ReturnsTotalsAndPercent()
        {
            int courseId = CreateCourseWithAttendanceAndGrades();

            List<AttendanceReportLineBE> result = _reportMPP.GetAttendanceReport(courseId);

            Assert.AreEqual(2, result.Count);

            AttendanceReportLineBE juan = result.Find(l => l.StudentId == 1);
            Assert.AreEqual(3, juan.TotalPlans);
            Assert.AreEqual(2, juan.PresentPlans);
            Assert.AreEqual(67, juan.AttendancePercent);

            AttendanceReportLineBE maria = result.Find(l => l.StudentId == 2);
            Assert.AreEqual(3, maria.TotalPlans);
            Assert.AreEqual(1, maria.PresentPlans);
            Assert.AreEqual(33, maria.AttendancePercent);
        }

        [TestMethod]
        public void GetAttendanceReport_NoEnrollments_ReturnsEmptyList()
        {
            List<AttendanceReportLineBE> result = _reportMPP.GetAttendanceReport(99999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetGradesReport_ReturnsCountersAndAverage()
        {
            int courseId = CreateCourseWithAttendanceAndGrades();

            List<GradeReportLineBE> result = _reportMPP.GetGradesReport(courseId);

            Assert.AreEqual(2, result.Count);

            GradeReportLineBE juan = result.Find(l => l.StudentId == 1);
            Assert.AreEqual(2, juan.EvaluationsCount);
            Assert.AreEqual(2, juan.GradesCount);
            Assert.AreEqual(9m, juan.Average);

            GradeReportLineBE maria = result.Find(l => l.StudentId == 2);
            Assert.AreEqual(2, maria.EvaluationsCount);
            Assert.AreEqual(0, maria.GradesCount);
            Assert.IsNull(maria.Average);
        }

        [TestMethod]
        public void GetGradesReport_WithoutEvaluations_ReturnsZeroEvaluations()
        {
            int courseId = CreateCourseWithStudentOnly();

            List<GradeReportLineBE> result = _reportMPP.GetGradesReport(courseId);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0, result[0].EvaluationsCount);
            Assert.AreEqual(0, result[0].GradesCount);
            Assert.IsNull(result[0].Average);
        }
    }
}
