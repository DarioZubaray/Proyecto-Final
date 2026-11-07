using System;
using System.IO;
using System.Windows.Forms;

using BE.Properties;
using BLL.Helpers;
using DAL;

namespace TrabajoFinal_DarioZubaray
{
    static class Program
    {
        private const int ExitOk = 0;
        private const int ExitUsage = 1;
        private const int ExitConnectionFailed = 2;
        private const int ExitBootstrapFailed = 3;

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                return RunCommandLine(args);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            CultureHelperBLL.SetCulture(AppPreferencesBLL.LastLanguage);

            Form startupForm = CreateStartupForm();

            if (startupForm == null)
            {
                return ExitOk;
            }

            Application.Run(startupForm);
            return ExitOk;
        }

        private static Form CreateStartupForm()
        {
            while (true)
            {
                try
                {
                    if (ServiceLocatorBLL.CreateAuthBLL().TestConnection())
                    {
                        bool hasRegisteredUsers = ServiceLocatorBLL
                            .CreateUserBLL()
                            .HasRegisteredUsers();

                        return hasRegisteredUsers
                            ? (Form)new LoginForm()
                            : new RegisterForm();
                    }
                }
                catch
                {
                    // Conexion caida o esquema ausente: se trata como DB-001.
                }

                DialogResult decision = MessageBox.Show(
                    string.Format(Resources.Auth_DbUnavailable, ErrorCodesBLL.Database.Unavailable),
                    Resources.Main_Title,
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error);

                if (decision != DialogResult.Retry)
                {
                    return null;
                }
            }
        }

        #region Modo consola (instalador)
        private static int RunCommandLine(string[] args)
        {
            string command = args[0];

            if (string.Equals(command, "--test-connection", StringComparison.OrdinalIgnoreCase))
            {
                return CliTestConnection();
            }

            if (string.Equals(command, "--bootstrap", StringComparison.OrdinalIgnoreCase))
            {
                return CliBootstrap(GetOption(args, "--sql-dir"));
            }

            CliLog("Argumento desconocido: " + command);
            return ExitUsage;
        }

        private static string GetOption(string[] args, string name)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static int CliTestConnection()
        {
            try
            {
                bool ok = new AccessDAL().TestConnection();
                CliLog("--test-connection: " + (ok ? "OK" : "FAIL"));
                return ok ? ExitOk : ExitConnectionFailed;
            }
            catch (Exception ex)
            {
                CliLog("--test-connection: FAIL - " + ex.Message);
                return ExitConnectionFailed;
            }
        }

        private static int CliBootstrap(string sqlDir)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlDir))
                {
                    sqlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sql");
                }

                if (!Directory.Exists(sqlDir))
                {
                    CliLog("--bootstrap: FAIL - directorio SQL inexistente: " + sqlDir);
                    return ExitBootstrapFailed;
                }

                // Cadena de instalacion: todos los scripts NN_*.sql, excepto
                // los de desarrollo/demo (02_* y "Consolidado") y los
                // de reversa ("Rollback").
                string[] files = Directory.GetFiles(sqlDir, "*.sql");
                Array.Sort(files, StringComparer.OrdinalIgnoreCase);

                var runner = new SqlScriptRunner();
                runner.EnsureDatabase();
                CliLog("--bootstrap: base verificada");
                int applied = 0;

                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);

                    if (name.StartsWith("02_", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (name.IndexOf("Rollback", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    if (name.IndexOf("Consolidado", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        continue;
                    }

                    try
                    {
                        int batches = runner.RunFile(file);
                        applied++;
                        CliLog("--bootstrap: OK " + name + " (" + batches + " lotes)");
                    }
                    catch (Exception ex)
                    {
                        CliLog("--bootstrap: FAIL " + name + " - " + ex.Message);
                        return ExitBootstrapFailed;
                    }
                }

                if (applied == 0)
                {
                    CliLog("--bootstrap: FAIL - sin scripts en " + sqlDir);
                    return ExitBootstrapFailed;
                }

                CliLog("--bootstrap: OK (" + applied + " scripts)");
                return ExitOk;
            }
            catch (Exception ex)
            {
                CliLog("--bootstrap: FAIL - " + ex.Message);
                return ExitBootstrapFailed;
            }
        }

        private static void CliLog(string message)
        {
            try
            {
                string path = Path.Combine(
                    Path.GetTempPath(),
                    "TrabajoFinal-DarioZubaray_bootstrap.log");

                File.AppendAllText(path,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " +
                    message + Environment.NewLine);
            }
            catch
            {
            }
        }
        #endregion

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ShowUnhandledError(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnhandledError(e.ExceptionObject as Exception);
        }

        private static void ShowUnhandledError(Exception exception)
        {
            string message = ErrorFormatter.WithCode(
                Resources.Global_UnhandledError,
                ErrorCodesBLL.General.Unhandled);

            MessageBox.Show(message,
                Resources.Main_Title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
