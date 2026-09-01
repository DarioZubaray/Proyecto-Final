using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BLL.Helpers;

namespace BLL.Tests
{
    [TestClass]
    public class AppPreferencesBLLTests
    {
        #region Campos
        private string _tempDirectory;
        private string _filePath;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Initialize()
        {
            _tempDirectory = Path.Combine(
                Path.GetTempPath(), "AppPreferencesBLLTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDirectory);
            _filePath = Path.Combine(_tempDirectory, "preferences.json");
            AppPreferencesBLL.SetFilePath(_filePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            AppPreferencesBLL.Reset();
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
            CultureHelperBLL.SetCulture(CultureHelperBLL.DefaultLanguage);
        }
        #endregion

        #region Tests
        [TestMethod]
        public void LastValues_NoFileSaved_ReturnDefaults()
        {
            Assert.AreEqual("es", AppPreferencesBLL.LastLanguage);
            Assert.AreEqual("System", AppPreferencesBLL.LastTheme);
        }

        [TestMethod]
        public void Save_PersistsValuesAcrossReload()
        {
            AppPreferencesBLL.Save("en", "Dark");

            AppPreferencesBLL.SetFilePath(_filePath);

            Assert.AreEqual("en", AppPreferencesBLL.LastLanguage);
            Assert.AreEqual("Dark", AppPreferencesBLL.LastTheme);
        }

        [TestMethod]
        public void Save_EmptyValues_FallBackToDefaults()
        {
            AppPreferencesBLL.Save("", null);

            Assert.AreEqual("es", AppPreferencesBLL.LastLanguage);
            Assert.AreEqual("System", AppPreferencesBLL.LastTheme);
        }

        [TestMethod]
        public void Save_UnsupportedLanguage_FallsBackToDefault()
        {
            AppPreferencesBLL.Save("zz", "Light");

            Assert.AreEqual("es", AppPreferencesBLL.LastLanguage);
            Assert.AreEqual("Light", AppPreferencesBLL.LastTheme);
        }

        [TestMethod]
        public void CorruptFile_ReturnsDefaults()
        {
            File.WriteAllText(_filePath, "{not valid json");

            AppPreferencesBLL.SetFilePath(_filePath);

            Assert.AreEqual("es", AppPreferencesBLL.LastLanguage);
            Assert.AreEqual("System", AppPreferencesBLL.LastTheme);
        }
        #endregion
    }
}