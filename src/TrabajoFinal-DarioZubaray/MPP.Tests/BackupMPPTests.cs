using System;
using System.IO;

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPP.Tests.Setup;

namespace MPP.Tests
{
    [TestClass]
    public class BackupMPPTests
    {
        private BackupMPP _backupMPP;
        private string _connectionString;
        private string _backupFilePath;

        [TestInitialize]
        public void Setup()
        {
            TestDatabaseHelper.EnsureDatabaseExists();
            TestDatabaseHelper.CreateSchema();
            TestDatabaseHelper.SeedTestData();
            _connectionString = TestDatabaseHelper.TestConnectionString;
            _backupMPP = new BackupMPP(_connectionString);
            _backupFilePath = Path.Combine(GetDefaultBackupDirectory(), "TF_DarioZubaray_TestBackup.bak");
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDatabaseHelper.CleanDatabase();
        }

        #region Métodos auxiliares
        private string GetDefaultBackupDirectory()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand("SELECT SERVERPROPERTY('InstanceDefaultBackupPath')", connection);
                object result = cmd.ExecuteScalar();
                return result as string ?? ".";
            }
        }

        private object ExecuteScalar(string sql)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(sql, connection);
                return cmd.ExecuteScalar();
            }
        }

        private void Execute(string sql)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
            }
        }

        private void SeedEvaluationsAndGrades()
        {
            Execute(@"INSERT INTO classrooms (name, capacity, is_active, created_at, last_update)
                      VALUES ('Aula Backup', 30, 1, GETDATE(), GETDATE());
                      INSERT INTO courses (name, start_date, end_date, classroom_id, day_of_week, start_time, end_time, is_active, created_at, last_update)
                      VALUES ('Curso Backup', '2026-08-01', '2026-12-31', 1, 1, '08:00', '10:00', 1, GETDATE(), GETDATE());
                      INSERT INTO evaluations (course_id, name, date, created_by, created_at, last_update) VALUES
                        (1, 'Parcial 1', '2026-09-15', 1, GETDATE(), GETDATE()),
                        (1, 'Parcial 2', '2026-10-20', 1, GETDATE(), GETDATE());
                      INSERT INTO grades (evaluation_id, student_id, grade) VALUES (1, 1, 8), (2, 2, 10);");
        }
        #endregion

        [TestMethod]
        public void CreateBackup_ReturnsTrue()
        {
            bool result = _backupMPP.CreateBackup(_backupFilePath);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CreateBackup_ProducesBackupReadableBySqlServer()
        {
            _backupMPP.CreateBackup(_backupFilePath);

            object fileCount = ExecuteScalar(
                $"RESTORE FILELISTONLY FROM DISK = '{_backupFilePath}'");

            Assert.IsNotNull(fileCount);
        }

        [TestMethod]
        public void RestoreBackup_RoundTripsData()
        {
            SeedEvaluationsAndGrades();
            _backupMPP.CreateBackup(_backupFilePath);

            Execute("DELETE FROM grades");
            object gradesAfterDelete = ExecuteScalar("SELECT COUNT(*) FROM grades");
            Assert.AreEqual(0, Convert.ToInt32(gradesAfterDelete));

            bool result = _backupMPP.RestoreBackup(_backupFilePath);

            Assert.IsTrue(result);
            object gradesAfterRestore = ExecuteScalar("SELECT COUNT(*) FROM grades");
            Assert.AreEqual(2, Convert.ToInt32(gradesAfterRestore));
        }

        [TestMethod]
        public void RestoreFromScript_ExecutesScriptAndLoadsData()
        {
            string scriptPath = Path.Combine(Path.GetTempPath(), "TF_DarioZubaray_TestSeed.sql");
            File.WriteAllText(scriptPath,
                "INSERT INTO classrooms (name, capacity, is_active, created_at, last_update) VALUES ('Aula Script', 25, 1, GETDATE(), GETDATE());" +
                Environment.NewLine + "GO",
                System.Text.Encoding.UTF8);

            try
            {
                bool result = _backupMPP.RestoreFromScript(scriptPath);

                Assert.IsTrue(result);
                object count = ExecuteScalar(
                    "SELECT COUNT(*) FROM classrooms WHERE name = 'Aula Script'");
                Assert.AreEqual(1, Convert.ToInt32(count));
            }
            finally
            {
                File.Delete(scriptPath);
            }
        }

        [TestMethod]
        public void RestoreFromScript_Throws_WhenScriptFileDoesNotExist()
        {
            string missingPath = Path.Combine(Path.GetTempPath(), "TF_DarioZubaray_NoExiste.sql");

            Assert.ThrowsException<FileNotFoundException>(
                () => _backupMPP.RestoreFromScript(missingPath));
        }
    }
}
