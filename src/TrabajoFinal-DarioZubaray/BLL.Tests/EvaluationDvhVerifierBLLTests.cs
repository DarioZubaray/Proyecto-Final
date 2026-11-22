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
    public class EvaluationDvhVerifierBLLTests
    {
        #region Propiedades
        private Mock<IEvaluationMPP> _mockEvaluationMPP;
        private EvaluationDvhVerifierBLL _verifier;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockEvaluationMPP = new Mock<IEvaluationMPP>();
            _verifier = new EvaluationDvhVerifierBLL(_mockEvaluationMPP.Object);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TableName_ReturnsEvaluations()
        {
            Assert.AreEqual("evaluations", _verifier.TableName);
        }

        [TestMethod]
        public void FindViolations_EvaluationWithValidDvh_ReturnsEmpty()
        {
            EvaluationBE evaluation = CreateEvaluation();
            evaluation.Dvh = DVHHelperBLL.Compute(evaluation.GetDvhValues());

            _mockEvaluationMPP.Setup(m => m.FindAll()).Returns(new List<EvaluationBE> { evaluation });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(0, violations.Count);
        }

        [TestMethod]
        public void FindViolations_EvaluationWithAlteredDvh_ReturnsViolation()
        {
            EvaluationBE evaluation = CreateEvaluation();
            evaluation.Dvh = "hash-viejo-que-no-coincide";

            _mockEvaluationMPP.Setup(m => m.FindAll()).Returns(new List<EvaluationBE> { evaluation });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual("evaluations", violations[0].TableName);
            Assert.AreEqual(evaluation.Id, violations[0].RowId);
            Assert.AreEqual(evaluation.Name, violations[0].Description);
            Assert.AreEqual("hash-viejo-que-no-coincide", violations[0].StoredHash);
            Assert.AreEqual(DVHHelperBLL.Compute(evaluation.GetDvhValues()), violations[0].ComputedHash);
        }

        [TestMethod]
        public void FindViolations_EvaluationWithoutDvh_ReturnsViolation()
        {
            EvaluationBE evaluation = CreateEvaluation();
            evaluation.Dvh = null;

            _mockEvaluationMPP.Setup(m => m.FindAll()).Returns(new List<EvaluationBE> { evaluation });

            List<IntegrityViolationBE> violations = _verifier.FindViolations();

            Assert.AreEqual(1, violations.Count);
            Assert.IsNull(violations[0].StoredHash);
        }

        [TestMethod]
        public void AcceptAndRegenerate_EvaluationFound_RegeneratesHash()
        {
            EvaluationBE evaluation = CreateEvaluation();
            evaluation.Dvh = "hash-alterado";

            _mockEvaluationMPP.Setup(m => m.FindById(evaluation.Id)).Returns(evaluation);
            _mockEvaluationMPP.Setup(m => m.UpdateDvh(evaluation.Id, It.IsAny<string>())).Returns(true);

            var violation = new IntegrityViolationBE
            {
                TableName = "evaluations",
                RowId = evaluation.Id
            };

            bool result = _verifier.AcceptAndRegenerate(violation);

            Assert.IsTrue(result);
            Assert.AreEqual(DVHHelperBLL.Compute(evaluation.GetDvhValues()), evaluation.Dvh);
            _mockEvaluationMPP.Verify(m => m.UpdateDvh(evaluation.Id, evaluation.Dvh), Times.Once);
        }

        [TestMethod]
        public void AcceptAndRegenerate_EvaluationNotFound_ReturnsFalse()
        {
            _mockEvaluationMPP.Setup(m => m.FindById(99)).Returns((EvaluationBE)null);

            var violation = new IntegrityViolationBE { TableName = "evaluations", RowId = 99 };

            Assert.IsFalse(_verifier.AcceptAndRegenerate(violation));
            _mockEvaluationMPP.Verify(m => m.UpdateDvh(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void AcceptAndRegenerate_NullOrInvalid_ReturnsFalse()
        {
            Assert.IsFalse(_verifier.AcceptAndRegenerate(null));
            Assert.IsFalse(_verifier.AcceptAndRegenerate(new IntegrityViolationBE { TableName = "evaluations", RowId = 0 }));
        }
        #endregion

        #region Helpers
        private EvaluationBE CreateEvaluation()
        {
            return new EvaluationBE
            {
                Id = 8,
                CourseId = 2,
                Name = "Parcial 2",
                Date = new System.DateTime(2026, 10, 20),
                CreatedBy = 3,
                CreatedAt = new System.DateTime(2026, 9, 1, 10, 0, 0),
                LastUpdate = new System.DateTime(2026, 9, 2, 10, 0, 0)
            };
        }
        #endregion
    }
}
