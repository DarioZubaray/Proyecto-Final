using System;
using System.IO;
using System.Threading.Tasks;
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

            Form startupForm = RunStartupWithSplash();

            if (startupForm == null)
            {
                // Verificacion fallida (servidor caido, instancia inexistente,
                // base inexistente o esquema ausente): se informa y se cierra.
                MessageBox.Show(
                    string.Format(
                        Resources.Auth_DbUnavailable,
                        ErrorCodesBLL.Database.Unavailable),
                    Resources.Main_Title,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return ExitConnectionFailed;
            }

            Application.Run(startupForm);
            return ExitOk;
        }

        /// <summary>
        /// Muestra el splash y verifica la base de datos en segundo plano,
        /// con el bucle de mensajes activo (la barra de progreso anima).
        /// Devuelve la primera pantalla (Login/Register) o null si la
        /// verificacion fallo.
        /// </summary>
        private static Form RunStartupWithSplash()
        {
            Form startupForm = null;
            DateTime startedAt = DateTime.MinValue;

            using (var splash = new SplashForm())
            {
                splash.Shown += async (sender, e) =>
                {
                    startedAt = DateTime.Now;

                    try
                    {
                        bool? hasRegisteredUsers =
                            await Task.Run(() => ProbeDatabase());

                        if (hasRegisteredUsers.HasValue)
                        {
                            // Se construye en el hilo de UI: los constructores
                            // tocan controles e inician verificaciones async.
                            startupForm = hasRegisteredUsers.Value
                                ? (Form)new LoginForm()
                                : new RegisterForm();
                        }
                    }
                    catch
                    {
                        startupForm = null;
                    }
                    finally
                    {
                        await EnsureMinimumSplashTimeAsync(startedAt);
                        splash.RequestClose();
                    }
                };

                Application.Run(splash);
            }

            return startupForm;
        }

        /// <summary>
        /// Sonda de arranque (corre en un hilo de fondo). Devuelve true si
        /// hay usuarios registrados, false si la base esta vacia, o null si
        /// la verificacion fallo (tratado como DB-001).
        /// </summary>
        private static bool? ProbeDatabase()
        {
            try
            {
                // AccessDAL relea la configuracion en cada intento.
                if (!new AccessDAL().TestConnection())
                {
                    return null;
                }

                return ServiceLocatorBLL.CreateUserBLL().HasRegisteredUsers();
            }
            catch
            {
                // Conexion caida, instancia inexistente, base inexistente
                // o esquema ausente: se trata como DB-001.
                return null;
            }
        }

        private static async Task EnsureMinimumSplashTimeAsync(DateTime startedAt)
        {
            const int MinimumSplashMilliseconds = 700;
            int elapsed = (int)(DateTime.Now - startedAt).TotalMilliseconds;

            if (elapsed < MinimumSplashMilliseconds)
            {
                await Task.Delay(MinimumSplashMilliseconds - elapsed);
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

                var runner = new SqlScriptRunner();
                runner.RunDirectory(sqlDir, message => CliLog("--bootstrap: " + message));
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
