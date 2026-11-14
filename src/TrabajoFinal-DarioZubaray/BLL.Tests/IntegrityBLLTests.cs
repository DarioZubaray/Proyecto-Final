using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BE.DTOs;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;
using Moq;

namespace BLL.Tests
{
    [TestClass]
    public class IntegrityBLLTests
    {
        #region Propiedades
        private Mock<IDvhTableVerifierBLL> _mockVerifier;
        private IntegrityBLL _integrityBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            IntegrityStateBLL.Reset();
            _mockVerifier = new Mock<IDvhTableVerifierBLL>();
            _mockVerifier.Setup(v => v.TableName).Returns("users");
            _integrityBLL = new IntegrityBLL(new[] { _mockVerifier.Object });
        }

        [TestCleanup]
        public void Cleanup()
        {
            IntegrityStateBLL.Reset();
        }
        #endregion

        #region Tests: Scan
        [TestMethod]
        public void Scan_NoViolations_RecordsCleanState()
        {
            _mockVerifier.Setup(v => v.FindViolations()).Returns(new List<IntegrityViolationBE>());

            List<IntegrityViolationBE> violations = _integrityBLL.Scan();

            Assert.AreEqual(0, violations.Count);
            Assert.IsTrue(IntegrityStateBLL.LastScanCompleted);
            Assert.IsFalse(IntegrityStateBLL.LastScanHasFailures);
        }

        [TestMethod]
        public void Scan_WithViolations_RecordsFailureState()
        {
            var violation = new IntegrityViolationBE { TableName = "users", RowId = 1 };
            _mockVerifier.Setup(v => v.FindViolations()).Returns(new List<IntegrityViolationBE> { violation });

            List<IntegrityViolationBE> violations = _integrityBLL.Scan();

            Assert.AreEqual(1, violations.Count);
            Assert.IsTrue(IntegrityStateBLL.LastScanHasFailures);
        }

        [TestMethod]
        public void Scan_AggregatesAllRegisteredVerifiers()
        {
            var otherVerifier = new Mock<IDvhTableVerifierBLL>();
            otherVerifier.Setup(v => v.TableName).Returns("courses");
            otherVerifier
                .Setup(v => v.FindViolations())
                .Returns(new List<IntegrityViolationBE>
                {
                    new IntegrityViolationBE { TableName = "courses", RowId = 5 }
                });

            _mockVerifier
                .Setup(v => v.FindViolations())
                .Returns(new List<IntegrityViolationBE>
                {
                    new IntegrityViolationBE { TableName = "users", RowId = 1 }
                });

            var integrityBLL = new IntegrityBLL(new[] { _mockVerifier.Object, otherVerifier.Object });

            List<IntegrityViolationBE> violations = integrityBLL.Scan();

            Assert.AreEqual(2, violations.Count);
            Assert.IsTrue(IntegrityStateBLL.LastScanHasFailures);
        }
        #endregion

        #region Tests: AcceptAndRegenerate
        [TestMethod]
        public void AcceptAndRegenerate_KnownTable_DelegatesAndRescans()
        {
            var violation = new IntegrityViolationBE { TableName = "users", RowId = 1 };
            _mockVerifier.Setup(v => v.AcceptAndRegenerate(violation)).Returns(true);
            _mockVerifier.Setup(v => v.FindViolations()).Returns(new List<IntegrityViolationBE>());

            bool result = _integrityBLL.AcceptAndRegenerate(violation);

            Assert.IsTrue(result);
            _mockVerifier.Verify(v => v.AcceptAndRegenerate(violation), Times.Once);
            _mockVerifier.Verify(v => v.FindViolations(), Times.Once);
            Assert.IsFalse(IntegrityStateBLL.LastScanHasFailures);
        }

        [TestMethod]
        public void AcceptAndRegenerate_UnknownTable_ReturnsFalse()
        {
            var violation = new IntegrityViolationBE { TableName = "desconocida", RowId = 1 };

            bool result = _integrityBLL.AcceptAndRegenerate(violation);

            Assert.IsFalse(result);
            _mockVerifier.Verify(v => v.AcceptAndRegenerate(It.IsAny<IntegrityViolationBE>()), Times.Never);
        }

        [TestMethod]
        public void AcceptAndRegenerate_NullViolation_ReturnsFalse()
        {
            Assert.IsFalse(_integrityBLL.AcceptAndRegenerate(null));
        }
        #endregion
    }
}
