using System;
using System.Windows.Forms;

using BE.Properties;
using BLL.Helpers;

namespace TrabajoFinal_DarioZubaray
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new LoginForm());
        }

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
