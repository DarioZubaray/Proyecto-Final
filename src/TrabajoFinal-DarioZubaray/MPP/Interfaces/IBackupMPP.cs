namespace MPP.Interfaces
{
    public interface IBackupMPP
    {
        bool CreateBackup(string filePath);
        bool RestoreBackup(string filePath);
        bool RestoreFromScript(string scriptPath);
    }
}
