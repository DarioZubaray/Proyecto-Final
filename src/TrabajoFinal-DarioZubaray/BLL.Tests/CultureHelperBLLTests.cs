using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BLL.Helpers;

namespace BLL.Tests
{
    [TestClass]
    public class CultureHelperBLLTests
    {
        #region Tests
        [TestMethod]
        public void SetCulture_SetsCorrectCulture()
        {
            CultureHelperBLL.SetCulture("en");

            Assert.AreEqual("en", Thread.CurrentThread.CurrentCulture.Name);
            Assert.AreEqual("en", Thread.CurrentThread.CurrentUICulture.Name);

            // Restaurar español
            CultureHelperBLL.SetCulture("es");
        }

        [TestMethod]
        public void SetCulture_InvalidCode_DefaultsToSpanish()
        {
            CultureHelperBLL.SetCulture("invalid-code");

            Assert.AreEqual("es", Thread.CurrentThread.CurrentCulture.Name);
            Assert.AreEqual("es", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [TestMethod]
        public void GetSupportedLanguages_Returns3Languages()
        {
            List<LanguageItemBLL> languages = CultureHelperBLL.GetSupportedLanguages();

            Assert.AreEqual(3, languages.Count);
            Assert.AreEqual("es", languages[0].Code);
            Assert.AreEqual("en", languages[1].Code);
            Assert.AreEqual("pt-BR", languages[2].Code);
        }
        #endregion
    }
}
