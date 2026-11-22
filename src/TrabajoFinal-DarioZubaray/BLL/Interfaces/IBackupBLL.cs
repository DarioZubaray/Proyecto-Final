namespace BLL.Interfaces
{
    public interface IBackupBLL
    {
        bool CreateBackup(string filePath);
        bool RestoreBackup(string filePath);
        bool RestoreFromScript(string scriptPath);
    }
}
