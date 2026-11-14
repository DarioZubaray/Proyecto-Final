using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.DTOs;
using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class GradeDvhVerifierBLLTests
    {
        #region Propiedades
        private Mock<IEvaluationMPP> _mockEvaluationMPP;
        private GradeDvhVerifierBLL _verifier;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockEvaluationMPP = new Mock<IEvaluationMPP>();
            _verifier = new GradeDvhVerifierBLL(_mockEvaluationMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TableName_ReturnsGrades()
        {
            Assert.AreEqual("grades", _verifier.TableName);
        }

        [TestMethod]
        public void FindViolations_GradeWithValidDvh_ReturnsEmpty()
        {
            GradeBE grade = CreateGrade();
            grade.Dvh = DVHHelperBLL.Compute(grade.GetDvhValues());

            _mockEvaluationMPP.Setup(m => m.FindAllGrades()).Returns(new List<GradeBE> { grade });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(0, violations.Count);
        }

        [TestMethod]
        public void FindViolations_GradeWithAlteredDvh_ReturnsViolation()
        {
            GradeBE grade = CreateGrade();
            grade.Dvh = "hash-viejo-que-no-coincide";

            _mockEvaluationMPP.Setup(m => m.FindAllGrades()).Returns(new List<GradeBE> { grade });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual("grades", violations[0].TableName);
            Assert.AreEqual(grade.Id, violations[0].RowId);
            StringAssert.Contains(violations[0].Description, "7.30");
            Assert.AreEqual("hash-viejo-que-no-coincide", violations[0].StoredHash);
            Assert.AreEqual(DVHHelperBLL.Compute(grade.GetDvhValues()), violations[0].ComputedHash);
        }

        [TestMethod]
        public void FindViolations_GradeWithoutDvh_ReturnsViolation()
        {
            GradeBE grade = CreateGrade();
            grade.Dvh = null;

            _mockEvaluationMPP.Setup(m => m.FindAllGrades()).Returns(new List<GradeBE> { grade });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.IsNull(violations[0].StoredHash);
        }

        [TestMethod]
        public void AcceptAndRegenerate_GradeFound_RegeneratesHash()
        {
            GradeBE grade = CreateGrade();
            grade.Dvh = "hash-alterado";

            _mockEvaluationMPP.Setup(m => m.FindGradeById(grade.Id)).Returns(grade);
            _mockEvaluationMPP.Setup(m => m.UpdateGradeDvh(grade.Id, It.IsAny<string>())).Returns(true);

            var violation = new IntegrityViolationBE
            {
                TableName = "grades",
                RowId = grade.Id
            };

            bool result = _verifier.AcceptAndRegenerate(violation);

            Assert.IsTrue(result);
            Assert.AreEqual(DVHHelperBLL.Compute(grade.GetDvhValues()), grade.Dvh);
            _mockEvaluationMPP.Verify(m => m.UpdateGradeDvh(grade.Id, grade.Dvh), Times.Once);
        }

        [TestMethod]
        public void AcceptAndRegenerate_GradeNotFound_ReturnsFalse()
        {
            _mockEvaluationMPP.Setup(m => m.FindGradeById(99)).Returns((GradeBE)null);

            var violation = new IntegrityViolationBE { TableName = "grades", RowId = 99 };

            Assert.IsFalse(_verifier.AcceptAndRegenerate(violation));
            _mockEvaluationMPP.Verify(m => m.UpdateGradeDvh(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void AcceptAndRegenerate_NullOrInvalid_ReturnsFalse()
        {
            Assert.IsFalse(_verifier.AcceptAndRegenerate(null));
            Assert.IsFalse(_verifier.AcceptAndRegenerate(new IntegrityViolationBE { TableName = "grades", RowId = 0 }));
        }
        #endregion

        #region Helpers
        private GradeBE CreateGrade()
        {
            return new GradeBE
            {
                Id = 12,
                EvaluationId = 8,
                StudentId = 4,
                Grade = 7.30m
            };
        }
        #endregion
    }
}
