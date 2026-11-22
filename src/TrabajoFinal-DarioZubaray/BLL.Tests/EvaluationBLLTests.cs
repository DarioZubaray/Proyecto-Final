using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.Entities;
using BLL.Helpers;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class EvaluationBLLTests
    {
        #region Propiedades
        private Mock<IEvaluationMPP> _mockEvaluationMPP;
        private Mock<ICourseMPP> _mockCourseMPP;
        private EvaluationBLL _evaluationBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockEvaluationMPP = new Mock<IEvaluationMPP>();
            _mockCourseMPP = new Mock<ICourseMPP>();
            _evaluationBLL = new EvaluationBLL(_mockEvaluationMPP.Object, _mockCourseMPP.Object);
        }
        #endregion

        #region Métodos auxiliares
        private EvaluationBE CreateValidEvaluation()
        {
            return new EvaluationBE
            {
                CourseId = 10,
                Name = "Parcial 1",
                Date = new DateTime(2026, 9, 15),
                CreatedBy = 2
            };
        }

        private void SetupTeacherCourses(params int[] courseIds)
        {
            var courses = new List<CourseBE>();
            foreach (int courseId in courseIds)
            {
                courses.Add(new CourseBE
                {
                    Id = courseId,
                    Name = "Curso " + courseId,
                    StartDate = new DateTime(2026, 1, 1),
                    EndDate = new DateTime(2026, 12, 31)
                });
            }

            _mockCourseMPP.Setup(m => m.FindByTeacherId(2)).Returns(courses);
        }
        #endregion

        #region Tests - Save
        [TestMethod]
        public void Save_NullEvaluation_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _evaluationBLL.Save(null, new List<GradeBE>()));
        }

        [TestMethod]
        public void Save_EmptyName_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            evaluation.Name = "  ";

            Assert.ThrowsException<InvalidOperationException>(() => _evaluationBLL.Save(evaluation, new List<GradeBE>()));
        }

        [TestMethod]
        public void Save_GradeOutOfRange_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            var grades = new List<GradeBE>
            {
                new GradeBE { StudentId = 1, Grade = 11 }
            };

            Assert.ThrowsException<InvalidOperationException>(() => _evaluationBLL.Save(evaluation, grades));
        }

        [TestMethod]
        public void Save_NegativeGrade_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            var grades = new List<GradeBE>
            {
                new GradeBE { StudentId = 1, Grade = -1 }
            };

            Assert.ThrowsException<InvalidOperationException>(() => _evaluationBLL.Save(evaluation, grades));
        }

        [TestMethod]
        public void Save_CourseNotOwnedByTeacher_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(99);

            Assert.ThrowsException<InvalidOperationException>(() => _evaluationBLL.Save(evaluation, new List<GradeBE>()));
        }

        [TestMethod]
        public void Save_DateBeforeCourseStart_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            evaluation.Date = new DateTime(2025, 12, 31);
            SetupTeacherCourses(10);

            InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(
                () => _evaluationBLL.Save(evaluation, new List<GradeBE>()));

            Assert.IsTrue(ex.Message.Contains(ErrorCodesBLL.Business.EvaluationDateOutOfRange));
            _mockEvaluationMPP.Verify(m => m.Save(It.IsAny<EvaluationBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_DateAfterCourseEnd_Throws()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            evaluation.Date = new DateTime(2027, 1, 1);
            SetupTeacherCourses(10);

            InvalidOperationException ex = Assert.ThrowsException<InvalidOperationException>(
                () => _evaluationBLL.Save(evaluation, new List<GradeBE>()));

            Assert.IsTrue(ex.Message.Contains(ErrorCodesBLL.Business.EvaluationDateOutOfRange));
            _mockEvaluationMPP.Verify(m => m.Save(It.IsAny<EvaluationBE>()), Times.Never);
        }

        [TestMethod]
        public void Save_DateOnCourseStartBoundary_ReturnsTrue()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            evaluation.Date = new DateTime(2026, 1, 1);
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);

            bool result = _evaluationBLL.Save(evaluation, new List<GradeBE>());

            Assert.IsTrue(result);
            _mockEvaluationMPP.Verify(m => m.Save(It.IsAny<EvaluationBE>()), Times.Once);
        }

        [TestMethod]
        public void Save_DateOnCourseEndBoundary_ReturnsTrue()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            evaluation.Date = new DateTime(2026, 12, 31);
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);

            bool result = _evaluationBLL.Save(evaluation, new List<GradeBE>());

            Assert.IsTrue(result);
            _mockEvaluationMPP.Verify(m => m.Save(It.IsAny<EvaluationBE>()), Times.Once);
        }

        [TestMethod]
        public void Save_Valid_ReturnsTrueAndSavesGrades()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);
            var grades = new List<GradeBE>
            {
                new GradeBE { StudentId = 1, Grade = 7 },
                new GradeBE { StudentId = 2, Grade = 9.5m }
            };

            bool result = _evaluationBLL.Save(evaluation, grades);

            Assert.IsTrue(result);
            Assert.AreEqual(55, evaluation.Id);
            _mockEvaluationMPP.Verify(m => m.Save(It.IsAny<EvaluationBE>()), Times.Once);
            _mockEvaluationMPP.Verify(m => m.SaveGrades(55, grades), Times.Once);
            Assert.AreEqual(55, grades[0].EvaluationId);
            Assert.AreEqual(55, grades[1].EvaluationId);
        }

        [TestMethod]
        public void Save_MppFails_ReturnsFalse()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(0);

            bool result = _evaluationBLL.Save(evaluation, new List<GradeBE>());

            Assert.IsFalse(result);
            _mockEvaluationMPP.Verify(m => m.SaveGrades(It.IsAny<int>(), It.IsAny<List<GradeBE>>()), Times.Never);
        }
        #endregion

        #region Tests - FindByCourseId / FindGradesByEvaluationId
        [TestMethod]
        public void FindByCourseId_DelegatesToMPP()
        {
            var expected = new List<EvaluationBE>
            {
                new EvaluationBE { Id = 1, CourseId = 10, Name = "Parcial 1" }
            };
            _mockEvaluationMPP.Setup(m => m.FindByCourseId(10)).Returns(expected);

            List<EvaluationBE> result = _evaluationBLL.FindByCourseId(10);

            Assert.AreEqual(1, result.Count);
            _mockEvaluationMPP.Verify(m => m.FindByCourseId(10), Times.Once);
        }

        [TestMethod]
        public void FindGradesByEvaluationId_DelegatesToMPP()
        {
            var expected = new List<GradeBE>
            {
                new GradeBE { Id = 1, EvaluationId = 5, StudentId = 10, Grade = 8 }
            };
            _mockEvaluationMPP.Setup(m => m.FindGradesByEvaluationId(5)).Returns(expected);

            List<GradeBE> result = _evaluationBLL.FindGradesByEvaluationId(5);

            Assert.AreEqual(1, result.Count);
            _mockEvaluationMPP.Verify(m => m.FindGradesByEvaluationId(5), Times.Once);
        }
        #endregion

        #region Tests - DVH
        [TestMethod]
        public void Save_RegeneratesEvaluationAndGradeDvh()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);

            EvaluationBE persisted = CreateValidEvaluation();
            persisted.Id = 55;
            _mockEvaluationMPP.Setup(m => m.FindById(55)).Returns(persisted);
            _mockEvaluationMPP.Setup(m => m.UpdateDvh(55, It.IsAny<string>())).Returns(true);

            GradeBE persistedGrade = new GradeBE
            {
                Id = 7,
                EvaluationId = 55,
                StudentId = 1,
                Grade = 7.3m
            };
            _mockEvaluationMPP.Setup(m => m.FindGradesByEvaluationId(55))
                .Returns(new List<GradeBE> { persistedGrade });
            _mockEvaluationMPP.Setup(m => m.UpdateGradeDvh(7, It.IsAny<string>())).Returns(true);

            var grades = new List<GradeBE>
            {
                new GradeBE { StudentId = 1, Grade = 7.3m }
            };

            bool result = _evaluationBLL.Save(evaluation, grades);

            Assert.IsTrue(result);
            _mockEvaluationMPP.Verify(
                m => m.UpdateDvh(55, DVHHelperBLL.Compute(persisted.GetDvhValues())),
                Times.Once);
            _mockEvaluationMPP.Verify(
                m => m.UpdateGradeDvh(7, DVHHelperBLL.Compute(persistedGrade.GetDvhValues())),
                Times.Once);
        }

        [TestMethod]
        public void Save_NoGrades_StillRegeneratesEvaluationDvh()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);

            EvaluationBE persisted = CreateValidEvaluation();
            persisted.Id = 55;
            _mockEvaluationMPP.Setup(m => m.FindById(55)).Returns(persisted);
            _mockEvaluationMPP.Setup(m => m.UpdateDvh(55, It.IsAny<string>())).Returns(true);
            _mockEvaluationMPP.Setup(m => m.FindGradesByEvaluationId(55))
                .Returns(new List<GradeBE>());

            bool result = _evaluationBLL.Save(evaluation, new List<GradeBE>());

            Assert.IsTrue(result);
            _mockEvaluationMPP.Verify(
                m => m.UpdateDvh(55, DVHHelperBLL.Compute(persisted.GetDvhValues())),
                Times.Once);
            _mockEvaluationMPP.Verify(
                m => m.UpdateGradeDvh(It.IsAny<int>(), It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void Save_EvaluationDvhUpdateFails_ReturnsFalse()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);
            EvaluationBE persisted = CreateValidEvaluation();
            persisted.Id = 55;
            _mockEvaluationMPP.Setup(m => m.FindById(55)).Returns(persisted);
            _mockEvaluationMPP.Setup(m => m.UpdateDvh(55, It.IsAny<string>())).Returns(false);

            bool result = _evaluationBLL.Save(evaluation, new List<GradeBE>());

            Assert.IsFalse(result);
            _mockEvaluationMPP.Verify(m => m.FindGradesByEvaluationId(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void Save_GradeDvhUpdateFails_ReturnsFalse()
        {
            EvaluationBE evaluation = CreateValidEvaluation();
            SetupTeacherCourses(10);
            _mockEvaluationMPP.Setup(m => m.Save(It.IsAny<EvaluationBE>())).Returns(55);
            EvaluationBE persistedForGrade = CreateValidEvaluation();
            persistedForGrade.Id = 55;
            _mockEvaluationMPP.Setup(m => m.FindById(55)).Returns(persistedForGrade);
            _mockEvaluationMPP.Setup(m => m.UpdateDvh(55, It.IsAny<string>())).Returns(true);

            GradeBE persistedGrade = new GradeBE { Id = 7, EvaluationId = 55, StudentId = 1, Grade = 8 };
            _mockEvaluationMPP.Setup(m => m.FindGradesByEvaluationId(55))
                .Returns(new List<GradeBE> { persistedGrade });
            _mockEvaluationMPP.Setup(m => m.UpdateGradeDvh(7, It.IsAny<string>())).Returns(false);

            var grades = new List<GradeBE> { new GradeBE { StudentId = 1, Grade = 8 } };

            bool result = _evaluationBLL.Save(evaluation, grades);

            Assert.IsFalse(result);
        }
        #endregion
    }
}
