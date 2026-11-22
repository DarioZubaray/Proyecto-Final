using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using BLL.Services;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class EvaluationDvhIntegrationTests
    {
        #region Propiedades
        private ClassroomMPP _classroomMPP;
        private CourseMPP _courseMPP;
        private EvaluationMPP _evaluationMPP;
        private EvaluationBLL _evaluationBLL;
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
            _evaluationMPP = new EvaluationMPP(_connectionString);
            _evaluationBLL = new EvaluationBLL(_evaluationMPP, _courseMPP);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }
        #endregion

        #region Métodos
        private int CreateTestCourse()
        {
            var classroom = new ClassroomBE
            {
                Name = "Aula DVH Evaluaciones",
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
                Name = "Curso DVH Evaluaciones",
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

        private int CreateTestTeacher(int courseId)
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var insertUser = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO users (user_name, password_hash, is_active, retries_count, last_update, created_at, language, theme, role_id)
                      VALUES ('dvh_eval_teacher', 'hash', 1, 0, GETDATE(), GETDATE(), 'es', 'System', NULL);
                      SELECT SCOPE_IDENTITY();", connection);
                int teacherId = Convert.ToInt32(insertUser.ExecuteScalar());

                var linkCourse = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO course_teachers (course_id, teacher_id, is_active, created_at)
                      VALUES (@courseId, @teacherId, 1, GETDATE())", connection);
                linkCourse.Parameters.AddWithValue("@courseId", courseId);
                linkCourse.Parameters.AddWithValue("@teacherId", teacherId);
                linkCourse.ExecuteNonQuery();

                return teacherId;
            }
        }

        private int CreateTestStudent(string document)
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    @"INSERT INTO students (first_name, last_name, document, email, phone, is_active, created_at, last_update)
                      VALUES ('Test', 'Dvh', @document, NULL, NULL, 1, GETDATE(), GETDATE());
                      SELECT SCOPE_IDENTITY();", connection);
                cmd.Parameters.AddWithValue("@document", document);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private EvaluationBE NewEvaluation(int courseId, int teacherId, string name, DateTime date)
        {
            return new EvaluationBE
            {
                CourseId = courseId,
                Name = name,
                Date = date,
                CreatedBy = teacherId,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now
            };
        }
        #endregion

        #region Tests
        [TestMethod]
        public void Save_NewEvaluationWithGrades_RegeneratesDvh()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);
            int studentId = CreateTestStudent("DNI-DVH-1");

            EvaluationBE evaluation = NewEvaluation(courseId, teacherId, "Parcial DVH", new DateTime(2026, 9, 15));
            var grades = new List<GradeBE>
            {
                new GradeBE { StudentId = studentId, Grade = 7.3m }
            };

            Assert.IsTrue(_evaluationBLL.Save(evaluation, grades));

            EvaluationBE persisted = _evaluationMPP.FindById(evaluation.Id);
            Assert.IsNotNull(persisted);
            Assert.IsFalse(string.IsNullOrEmpty(persisted.Dvh));
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);

            List<GradeBE> persistedGrades = _evaluationMPP.FindGradesByEvaluationId(evaluation.Id);
            Assert.AreEqual(1, persistedGrades.Count);
            Assert.IsFalse(string.IsNullOrEmpty(persistedGrades[0].Dvh));
            Assert.AreEqual(DVHHelperBLL.Compute(persistedGrades[0].GetDvhValues()), persistedGrades[0].Dvh);
        }

        [TestMethod]
        public void Save_ExistingEvaluation_RegeneratesDvh()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);

            int evaluationId = _evaluationMPP.Save(
                NewEvaluation(courseId, teacherId, "Nombre viejo", new DateTime(2026, 10, 1)));

            EvaluationBE evaluation = _evaluationMPP.FindById(evaluationId);
            Assert.IsNotNull(evaluation);

            evaluation.Name = "Nombre nuevo";
            Assert.IsTrue(_evaluationBLL.Save(evaluation, new List<GradeBE>()));

            EvaluationBE persisted = _evaluationMPP.FindById(evaluationId);

            Assert.IsNotNull(persisted);
            Assert.AreEqual("Nombre nuevo", persisted.Name);
            Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh);
        }

        [TestMethod]
        public void AlteredEvaluationOutOfBand_StoredDvhDoesNotMatch()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);

            EvaluationBE evaluation = NewEvaluation(courseId, teacherId, "Parcial Intacto", new DateTime(2026, 9, 20));
            Assert.IsTrue(_evaluationBLL.Save(evaluation, new List<GradeBE>()));

            ExecuteNonQuery("UPDATE evaluations SET name = 'alterado' WHERE id = " + evaluation.Id);

            EvaluationBE altered = _evaluationMPP.FindById(evaluation.Id);

            Assert.IsNotNull(altered);
            Assert.AreNotEqual(DVHHelperBLL.Compute(altered.GetDvhValues()), altered.Dvh);
        }

        [TestMethod]
        public void AlteredGradeOutOfBand_StoredDvhDoesNotMatch()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);
            int studentId = CreateTestStudent("DNI-DVH-2");

            EvaluationBE evaluation = NewEvaluation(courseId, teacherId, "Parcial Notas", new DateTime(2026, 9, 22));
            Assert.IsTrue(_evaluationBLL.Save(evaluation, new List<GradeBE>
            {
                new GradeBE { StudentId = studentId, Grade = 6.5m }
            }));

            GradeBE grade = _evaluationMPP.FindGradesByEvaluationId(evaluation.Id)[0];
            Assert.AreEqual(DVHHelperBLL.Compute(grade.GetDvhValues()), grade.Dvh);

            ExecuteNonQuery("UPDATE grades SET grade = 9.90 WHERE id = " + grade.Id);

            GradeBE altered = _evaluationMPP.FindGradeById(grade.Id);

            Assert.IsNotNull(altered);
            Assert.AreNotEqual(DVHHelperBLL.Compute(altered.GetDvhValues()), altered.Dvh);
        }

        [TestMethod]
        public void Verifier_DetectsAndAcceptAndRegeneratesTamperedRows()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);
            int studentId = CreateTestStudent("DNI-DVH-3");

            EvaluationBE evaluation = NewEvaluation(courseId, teacherId, "Parcial Verificador", new DateTime(2026, 9, 25));
            Assert.IsTrue(_evaluationBLL.Save(evaluation, new List<GradeBE>
            {
                new GradeBE { StudentId = studentId, Grade = 8.25m }
            }));

            ExecuteNonQuery("UPDATE evaluations SET name = 'hackeado' WHERE id = " + evaluation.Id);
            GradeBE grade = _evaluationMPP.FindGradesByEvaluationId(evaluation.Id)[0];
            ExecuteNonQuery("UPDATE grades SET grade = 1.00 WHERE id = " + grade.Id);

            var evaluationVerifier = new EvaluationDvhVerifierBLL(_evaluationMPP);
            var gradeVerifier = new GradeDvhVerifierBLL(_evaluationMPP);

            List<BE.DTOs.IntegrityViolationBE> evaluationViolations = evaluationVerifier.FindViolations();
            List<BE.DTOs.IntegrityViolationBE> gradeViolations = gradeVerifier.FindViolations();

            Assert.AreEqual(1, evaluationViolations.Count);
            Assert.AreEqual(evaluation.Id, evaluationViolations[0].RowId);
            Assert.AreEqual(1, gradeViolations.Count);
            Assert.AreEqual(grade.Id, gradeViolations[0].RowId);

            Assert.IsTrue(evaluationVerifier.AcceptAndRegenerate(evaluationViolations[0]));
            Assert.IsTrue(gradeVerifier.AcceptAndRegenerate(gradeViolations[0]));

            Assert.AreEqual(0, evaluationVerifier.FindViolations().Count);
            Assert.AreEqual(0, gradeVerifier.FindViolations().Count);

            EvaluationBE repairedEvaluation = _evaluationMPP.FindById(evaluation.Id);
            GradeBE repairedGrade = _evaluationMPP.FindGradeById(grade.Id);

            Assert.AreEqual(DVHHelperBLL.Compute(repairedEvaluation.GetDvhValues()), repairedEvaluation.Dvh);
            Assert.AreEqual(DVHHelperBLL.Compute(repairedGrade.GetDvhValues()), repairedGrade.Dvh);
        }

        [TestMethod]
        public void Dvh_BackfillExpression_MatchesApplicationHash()
        {
            int courseId = CreateTestCourse();
            int teacherId = CreateTestTeacher(courseId);
            int studentA = CreateTestStudent("DNI-DVH-P1");
            int studentB = CreateTestStudent("DNI-DVH-P2");

            DateTime[] stamps = new DateTime[]
            {
                new DateTime(2026, 10, 9, 13, 45, 59, 998),
                new DateTime(2026, 10, 9, 13, 45, 59, 0),
                new DateTime(2026, 1, 2, 8, 30, 0, 123),
                new DateTime(2026, 6, 15, 23, 59, 59, 0)
            };

            var evaluationIds = new List<int>();

            for (int i = 0; i < stamps.Length; i++)
            {
                EvaluationBE evaluation = NewEvaluation(
                    courseId, teacherId, "Parcial Paridad " + i, new DateTime(2026, 9, 10 + i));
                evaluation.CreatedAt = stamps[i];
                evaluation.LastUpdate = stamps[i];

                int evaluationId = _evaluationMPP.Save(evaluation);
                evaluationIds.Add(evaluationId);

                _evaluationMPP.SaveGrades(evaluationId, new List<GradeBE>
                {
                    new GradeBE { StudentId = studentA, Grade = 7.3m },
                    new GradeBE { StudentId = studentB, Grade = 10.0m }
                });
            }

            ExecuteNonQuery(@"
                UPDATE e
                SET e.[dvh] = LOWER(CONVERT(VARCHAR(64),
                    HASHBYTES('SHA2_256',
                        CONCAT(
                            CONVERT(NVARCHAR(MAX), e.[id]),                ',',
                            CONVERT(NVARCHAR(MAX), e.[course_id]),         ',',
                            e.[name],                                      ',',
                            CONVERT(NVARCHAR(30), CAST(e.[date] AS DATETIME), 126), ',',
                            CONVERT(NVARCHAR(MAX), e.[created_by]),        ',',
                            CONVERT(NVARCHAR(30), e.[created_at], 126),    ',',
                            CONVERT(NVARCHAR(30), e.[last_update], 126)
                        )
                    ), 2))
                FROM [dbo].[evaluations] e
                WHERE e.[dvh] IS NULL;

                UPDATE g
                SET g.[dvh] = LOWER(CONVERT(VARCHAR(64),
                    HASHBYTES('SHA2_256',
                        CONCAT(
                            CONVERT(NVARCHAR(MAX), g.[id]),             ',',
                            CONVERT(NVARCHAR(MAX), g.[evaluation_id]),  ',',
                            CONVERT(NVARCHAR(MAX), g.[student_id]),     ',',
                            CONVERT(NVARCHAR(MAX), g.[grade])
                        )
                    ), 2))
                FROM [dbo].[grades] g
                WHERE g.[dvh] IS NULL;");

            foreach (int evaluationId in evaluationIds)
            {
                EvaluationBE persisted = _evaluationMPP.FindById(evaluationId);

                Assert.IsNotNull(persisted);
                Assert.IsNotNull(persisted.Dvh);
                Assert.AreEqual(DVHHelperBLL.Compute(persisted.GetDvhValues()), persisted.Dvh,
                    "Paridad DVH fallida para evaluacion " + evaluationId);

                foreach (GradeBE grade in _evaluationMPP.FindGradesByEvaluationId(evaluationId))
                {
                    Assert.IsNotNull(grade.Dvh);
                    Assert.AreEqual(DVHHelperBLL.Compute(grade.GetDvhValues()), grade.Dvh,
                        "Paridad DVH fallida para nota " + grade.Id);
                }
            }
        }
        #endregion
    }
}
