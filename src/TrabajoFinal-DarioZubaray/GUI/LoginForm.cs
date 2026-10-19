using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using BE.DTOs;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        #region Propiedades
        private IAuthBLL _authBLL;
        private IIntegrityBLL _integrityBLL;
        #endregion

        #region Constructor
        public LoginForm()
        {
            InitializeComponent();
            _authBLL = ServiceLocatorBLL.CreateAuthBLL();
            _integrityBLL = ServiceLocatorBLL.CreateIntegrityBLL();
            CultureHelperBLL.SetCulture(AppPreferencesBLL.LastLanguage);
            ApplyResources();
            ThemeHelper.ApplyTheme(this, AppPreferencesBLL.LastTheme);
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

            if (connected)
            {
                await TryRunIntegrityScanAsync(true);
            }
        }

        private async Task TryRunIntegrityScanAsync(bool warnIfViolations)
        {
            try
            {
                await Task.Run(() => _integrityBLL.Scan());

                if (warnIfViolations && IntegrityStateBLL.LastScanHasFailures)
                {
                    MessageBox.Show(
                        Resources.Auth_IntegrityBlocked,
                        Resources.Integrity_Title,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch
            {
            }
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

        private async void btnLogin_Click(object sender, EventArgs e)
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

            await TryRunIntegrityScanAsync(false);

            LoginResultBE result = _authBLL.Login(username, password);

            if (result.Success && result.RequiresTwoFactor)
            {
                if (!PromptTwoFactor(result.User))
                {
                    return;
                }
            }

            if (result.Success)
            {
                RunIntegrityAuditIfRequired(result.User);

                LogLogin(result.User);
                SessionManagerBLL.CreateSession(result.User);
                AppPreferencesBLL.SavePreferences(result.User.Language, result.User.Theme);
                this.Hide();
                MainForm mainForm = new MainForm(result.User);
                DialogResult dialogResult = mainForm.ShowDialog();

                AppPreferencesBLL.SavePreferences(result.User.Language, result.User.Theme);
                SessionManagerBLL.RemoveSession(result.User.Id);
                LogLogout(result.User.Id, result.User.UserName);
                CultureHelperBLL.SetCulture(AppPreferencesBLL.LastLanguage);
                ApplyResources();
                ThemeHelper.ApplyTheme(this, AppPreferencesBLL.LastTheme);
                this.Show();
                txtUser.Text = "";
                txtUser.Focus();
                txtPass.Text = "";
                lblMessage.Visible = false;
                CheckDatabaseConnectionAsync();
            }
            else
            {
                lblMessage.Text = ErrorFormatter.WithCode(result.Message, result.ErrorCode);
                lblMessage.Visible = true;
            }
        }

        private void ShowDatabaseUnavailable()
        {
            lblMessage.Text = string.Format(Resources.Auth_DbUnavailable, ErrorCodesBLL.Database.Unavailable);
            lblMessage.Visible = true;
            SetDatabaseStatus(false);
        }

        private void RunIntegrityAuditIfRequired(UserBE user)
        {
            while (IntegrityStateBLL.LastScanHasFailures)
            {
                using (var form = new IntegrityAuditForm(user))
                {
                    form.ShowDialog(this);
                }

                if (!IntegrityStateBLL.LastScanHasFailures)
                {
                    return;
                }

                DialogResult decision = MessageBox.Show(
                    this,
                    Resources.Integrity_PendingContinueMessage,
                    Resources.Integrity_Title,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (decision == DialogResult.Yes)
                {
                    return;
                }
            }
        }

        private bool PromptTwoFactor(UserBE user)
        {
            while (true)
            {
                using (var form = new TwoFactorForm())
                {
                    if (form.ShowDialog(this) != DialogResult.OK)
                    {
                        lblMessage.Visible = false;
                        return false;
                    }

                    if (_authBLL.ValidateTwoFactor(user, form.Code))
                    {
                        lblMessage.Visible = false;
                        return true;
                    }

                    lblMessage.Text = ErrorFormatter.WithCode(
                        Resources.Auth_TwoFactorInvalid,
                        ErrorCodesBLL.Auth.TwoFactorInvalid);
                    lblMessage.Visible = true;
                }
            }
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
                ServiceLocatorBLL.CreateActivityBLL().LogLogin(user.Id, user.UserName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception LogLogin", e.Message);
            }
        }

        private void LogLogout(int userId, string userName)
        {
            try
            {
                ServiceLocatorBLL.CreateActivityBLL().LogLogout(userId, userName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception LogLogin", e.Message);
            }
        }
        #endregion
    }
}
