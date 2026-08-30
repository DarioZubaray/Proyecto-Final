using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        #region Constantes
        private const string DB_ERROR_CODE = "DB-001";
        #endregion

        #region Propiedades
        private IAuthBLL _authBLL;
        #endregion

        #region Constructor
        public LoginForm()
        {
            InitializeComponent();
            _authBLL = ServiceLocatorBLL.CreateAuthBLL();
            ApplyResources();
            CheckDatabaseConnectionAsync();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            groupBox1.Text = Resources.Login_Title;
            lblUser.Text = Resources.Login_UsernameLabel;
            lblPass.Text = Resources.Login_PasswordLabel;
            btnLogin.Text = Resources.Login_Button;
            lblDeveloper.Text = Resources.Login_Developer;
            lblMessage.Text = Resources.Login_MessageInvalid;
            lblDbStatus.Text = Resources.Login_DbChecking;
            lblDbStatus.ForeColor = System.Drawing.Color.DarkOrange;
            btnRetry.Text = Resources.Login_RetryButton;
        }

        private async void CheckDatabaseConnectionAsync()
        {
            SetCheckingStatus();
            bool connected = await Task.Run(() => TryTestConnection());
            SetDatabaseStatus(connected);
        }

        private bool TryTestConnection()
        {
            try
            {
                return _authBLL.TestConnection();
            }
            catch
            {
                return false;
            }
        }

        private void SetCheckingStatus()
        {
            lblDbStatus.Text = Resources.Login_DbChecking;
            lblDbStatus.ForeColor = System.Drawing.Color.DarkOrange;
            btnRetry.Visible = false;
        }

        private void SetDatabaseStatus(bool connected)
        {
            if (connected)
            {
                lblDbStatus.Text = Resources.Login_DbConnected;
                lblDbStatus.ForeColor = System.Drawing.Color.SeaGreen;
                btnRetry.Visible = false;
            }
            else
            {
                lblDbStatus.Text = Resources.Login_DbDisconnected;
                lblDbStatus.ForeColor = System.Drawing.Color.IndianRed;
                btnRetry.Visible = true;
            }
        }
        #endregion

        #region Eventos
        private async void btnRetry_Click(object sender, EventArgs e)
        {
            btnRetry.Enabled = false;
            SetCheckingStatus();
            bool connected = await Task.Run(() => TryTestConnection());
            SetDatabaseStatus(connected);
            btnRetry.Enabled = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            var username = txtUser.Text;
            var password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            if (!TryTestConnection())
            {
                ShowDatabaseUnavailable();
                return;
            }

            LoginResultBE result = _authBLL.Login(username, password);

            if (result.Success)
            {
                LogLogin(result.User);
                SessionManagerBLL.CreateSession(result.User);
                this.Hide();
                MainForm mainForm = new MainForm(result.User);
                DialogResult dialogResult = mainForm.ShowDialog();
                SessionManagerBLL.RemoveSession(result.User.Id);
                LogLogout(result.User.Id, result.User.UserName);
                CultureHelperBLL.SetCulture(CultureHelperBLL.DefaultLanguage);
                ApplyResources();
                this.Show();
                txtUser.Text = "";
                txtUser.Focus();
                txtPass.Text = "";
                lblMessage.Visible = false;
                CheckDatabaseConnectionAsync();
            }
            else
            {
                lblMessage.Text = result.Message;
                lblMessage.Visible = true;
            }
        }

        private void ShowDatabaseUnavailable()
        {
            lblMessage.Text = string.Format(Resources.Auth_DbUnavailable, DB_ERROR_CODE);
            lblMessage.Visible = true;
            SetDatabaseStatus(false);
        }

        private void txtUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPass.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        #endregion

        #region Historial de actividad
        private void LogLogin(UserBE user)
        {
            try
            {
                ServiceLocatorBLL.CreateActivityBLL()
                    .LogLogin(user.Id, user.UserName);
            }
            catch
            {
                // Loguear el acceso no debe interrumpir el flujo de login.
            }
        }

        private void LogLogout(int userId, string userName)
        {
            try
            {
                ServiceLocatorBLL.CreateActivityBLL()
                    .LogLogout(userId, userName);
            }
            catch
            {
                // Loguear el cierre no debe interrumpir el flujo.
            }
        }
        #endregion
    }
}
