using Microsoft.VisualStudio.TestTools.UnitTesting;

using BLL.Services;
using Moq;
using MPP.Interfaces;

namespace BLL.Tests
{
    [TestClass]
    public class BackupBLLTests
    {
        #region Propiedades
        private Mock<IBackupMPP> _mockBackupMPP;
        private BackupBLL _backupBLL;
        #endregion

        #region Inicialización
        [TestInitialize]
        public void Setup()
        {
            _mockBackupMPP = new Mock<IBackupMPP>();
            _backupBLL = new BackupBLL(_mockBackupMPP.Object);
        }
        #endregion

        [TestMethod]
        public void CreateBackup_DelegatesToMPP_AndReturnsTrue()
        {
            _mockBackupMPP.Setup(m => m.CreateBackup("C:\\backup\\respaldo.bak")).Returns(true);

            bool result = _backupBLL.CreateBackup("C:\\backup\\respaldo.bak");

            Assert.IsTrue(result);
            _mockBackupMPP.Verify(m => m.CreateBackup("C:\\backup\\respaldo.bak"), Times.Once);
        }

        [TestMethod]
        public void CreateBackup_ReturnsFalse_WhenMPPFails()
        {
            _mockBackupMPP.Setup(m => m.CreateBackup(It.IsAny<string>())).Returns(false);

            bool result = _backupBLL.CreateBackup("C:\\backup\\respaldo.bak");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RestoreBackup_DelegatesToMPP_AndReturnsTrue()
        {
            _mockBackupMPP.Setup(m => m.RestoreBackup("C:\\backup\\respaldo.bak")).Returns(true);

            bool result = _backupBLL.RestoreBackup("C:\\backup\\respaldo.bak");

            Assert.IsTrue(result);
            _mockBackupMPP.Verify(m => m.RestoreBackup("C:\\backup\\respaldo.bak"), Times.Once);
        }

        [TestMethod]
        public void RestoreBackup_ReturnsFalse_WhenMPPFails()
        {
            _mockBackupMPP.Setup(m => m.RestoreBackup(It.IsAny<string>())).Returns(false);

            bool result = _backupBLL.RestoreBackup("C:\\backup\\respaldo.bak");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RestoreFromScript_DelegatesToMPP_AndReturnsTrue()
        {
            _mockBackupMPP.Setup(m => m.RestoreFromScript("C:\\scripts\\seed.sql")).Returns(true);

            bool result = _backupBLL.RestoreFromScript("C:\\scripts\\seed.sql");

            Assert.IsTrue(result);
            _mockBackupMPP.Verify(m => m.RestoreFromScript("C:\\scripts\\seed.sql"), Times.Once);
        }

        [TestMethod]
        public void RestoreFromScript_ReturnsFalse_WhenMPPFails()
        {
            _mockBackupMPP.Setup(m => m.RestoreFromScript(It.IsAny<string>())).Returns(false);

            bool result = _backupBLL.RestoreFromScript("C:\\scripts\\seed.sql");

            Assert.IsFalse(result);
        }
    }
}
