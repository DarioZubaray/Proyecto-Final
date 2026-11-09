using BLL.Interfaces;
using MPP.Interfaces;

namespace BLL.Services
{
    public class BackupBLL : IBackupBLL
    {
        #region Propiedades
        private readonly IBackupMPP _backupMPP;
        #endregion

        #region Constructor
        public BackupBLL(IBackupMPP backupMPP)
        {
            _backupMPP = backupMPP;
        }
        #endregion

        #region Métodos
        public bool CreateBackup(string filePath)
        {
            return _backupMPP.CreateBackup(filePath);
        }

        public bool RestoreBackup(string filePath)
        {
            return _backupMPP.RestoreBackup(filePath);
        }

        public bool RestoreFromScript(string scriptPath)
        {
            return _backupMPP.RestoreFromScript(scriptPath);
        }
        #endregion
    }
}
