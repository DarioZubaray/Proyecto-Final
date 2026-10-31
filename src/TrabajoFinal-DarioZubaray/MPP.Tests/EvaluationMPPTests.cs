using System;
using System.Collections.Generic;
using BE.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class EvaluationMPPTests
    {
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private EvaluationMPP _evaluationMPP;
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
            _evaluationMPP = new EvaluationMPP(_connectionString);
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
                Name = "Aula Evaluaciones",
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
                Name = "Curso Evaluaciones",
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

        private int CreateTestTeacher()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('test_eval_teacher', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'System', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int CreateTestStudent()
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO students (first_name, last_name, document, email, phone, is_active, created_at, last_update)
                      VALUES ('Test', 'Evaluation', 'DNI-EVAL-1', NULL, NULL, 1, GETDATE(), GETDATE());
                      SELECT SCOPE_IDENTITY();", connection);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        public void Save_Insert_ReturnsNewId()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher();
            var evaluation = new EvaluationBE
            {
                CourseId = courseId,
                Name = "Parcial 1",
                Date = new DateTime(2026, 9, 15),
                CreatedBy = teacherId,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };

            int result = _evaluationMPP.Save(evaluation);

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void SaveGrades_InsertsAndFindGradesReturnsThem()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher();
            int studentId = CreateTestStudent();
            int evaluationId = _evaluationMPP.Save(new EvaluationBE
            {
                CourseId = courseId,
                Name = "Parcial 2",
                Date = new DateTime(2026, 10, 1),
                CreatedBy = teacherId,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            _evaluationMPP.SaveGrades(evaluationId, new List<GradeBE>
            {
                new GradeBE { StudentId = studentId, Grade = 8.5m }
            });

            List<GradeBE> grades = _evaluationMPP.FindGradesByEvaluationId(evaluationId);

            Assert.AreEqual(1, grades.Count);
            Assert.AreEqual(studentId, grades[0].StudentId);
            Assert.AreEqual(8.5m, grades[0].Grade);
        }

        [TestMethod]
        public void FindByCourseId_ReturnsEvaluations()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher();
            _evaluationMPP.Save(new EvaluationBE
            {
                CourseId = courseId,
                Name = "Final",
                Date = new DateTime(2026, 12, 10),
                CreatedBy = teacherId,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            });

            List<EvaluationBE> result = _evaluationMPP.FindByCourseId(courseId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 1);
            Assert.AreEqual("Final", result[0].Name);
        }

        [TestMethod]
        public void FindByCourseId_NoEvaluations_ReturnsEmptyList()
        {
            List<EvaluationBE> result = _evaluationMPP.FindByCourseId(99999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindGradesByEvaluationId_NoGrades_ReturnsEmptyList()
        {
            List<GradeBE> result = _evaluationMPP.FindGradesByEvaluationId(99999);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
