using System;
using System.Windows.Forms;

using BE.Properties;
using BLL.Helpers;

namespace TrabajoFinal_DarioZubaray
{
    /// <summary>
    /// Pantalla de inicio que se muestra mientras Program.cs verifica la
    /// conexion con la base de datos. Solo puede cerrarse mediante
    /// RequestClose(), cuando la verificacion finalizo.
    /// </summary>
    public partial class SplashForm : Form
    {
        #region Propiedades
        private bool _closeRequested;
        #endregion

        #region Constructor
        public SplashForm()
        {
            InitializeComponent();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, AppPreferencesBLL.LastTheme);
            TryAssignIcon();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            lblTitle.Text = Resources.Main_Title;
            lblStatus.Text = Resources.Login_DbChecking;
        }

        private void TryAssignIcon()
        {
            try
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Cierra el splash una vez finalizada la verificacion de la base
        /// de datos. Es el unico cierre permitido.
        /// </summary>
        public void RequestClose()
        {
            _closeRequested = true;
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!_closeRequested)
            {
                // Evita que el usuario cierre el splash mientras verifica:
                // la verificacion sigue corriendo en segundo plano.
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }
        #endregion
    }
}
