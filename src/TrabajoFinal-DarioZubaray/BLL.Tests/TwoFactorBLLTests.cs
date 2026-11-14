using Microsoft.VisualStudio.TestTools.UnitTesting;

using BLL.Services;

namespace BLL.Tests
{
    [TestClass]
    public class TwoFactorBLLTests
    {
        #region Propiedades
        private InternalTwoFactorBLL _twoFactor;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _twoFactor = new InternalTwoFactorBLL();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void ProviderName_IsInternal()
        {
            Assert.AreEqual("Internal", _twoFactor.ProviderName);
        }

        [TestMethod]
        public void Verify_ValidCode_ReturnsTrue()
        {
            Assert.IsTrue(_twoFactor.Verify("admin", "123"));
        }

        [TestMethod]
        public void Verify_InvalidCode_ReturnsFalse()
        {
            Assert.IsFalse(_twoFactor.Verify("admin", "456"));
            Assert.IsFalse(_twoFactor.Verify("admin", "abc"));
        }

        [TestMethod]
        public void Verify_EmptyOrNullCode_ReturnsFalse()
        {
            Assert.IsFalse(_twoFactor.Verify("admin", ""));
            Assert.IsFalse(_twoFactor.Verify("admin", null));
        }
        #endregion
    }
}
