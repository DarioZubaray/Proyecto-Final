using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;

namespace TrabajoFinal_DarioZubaray
{
    public partial class RegisterForm : Form
    {
        #region Propiedades
        private readonly IUserBLL _userBLL;
        private readonly IRoleBLL _roleBLL;
        #endregion

        #region Constructor
        public RegisterForm()
        {
            InitializeComponent();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _roleBLL = ServiceLocatorBLL.CreateRoleBLL();
            LoadLanguages();
            LoadThemes();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, AppPreferencesBLL.LastTheme);
        }
        #endregion

        #region Métodos
        private void LoadLanguages()
        {
            cbLanguage.DataSource = CultureHelperBLL.GetSupportedLanguages();
            cbLanguage.DisplayMember = "DisplayName";
            cbLanguage.ValueMember = "Code";
            cbLanguage.SelectedValue = CultureHelperBLL.DefaultLanguage;
        }

        private void LoadThemes()
        {
            cbTheme.DataSource = ThemeHelper.GetSupportedThemes();
            cbTheme.DisplayMember = "DisplayName";
            cbTheme.ValueMember = "Code";
            cbTheme.SelectedValue = ThemeHelper.DefaultTheme;
        }

        private void ApplyResources()
        {
            this.Text = Resources.Register_Title;
            lblUserName.Text = Resources.Login_UsernameLabel;
            lblPassword.Text = Resources.Login_PasswordLabel;
            lblConfirm.Text = Resources.Register_ConfirmLabel;
            lblLanguage.Text = Resources.Preferences_LanguageLabel;
            lblTheme.Text = Resources.Preferences_ThemeLabel;
            btnRegister.Text = Resources.Register_Button;
        }

        private bool ValidateInputs()
        {
            string userName = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.UserForm_UserNameRequired,
                    ErrorCodesBLL.Validation.UsernameRequired));
                txtUserName.Focus();
                return false;
            }

            // 'admin' es el usuario sembrado por la instalacion: queda reservado.
            if (UserBLL.IsAdminName(userName) || _userBLL.IsUsernameTaken(userName))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.UserForm_UserNameTaken,
                    ErrorCodesBLL.Validation.UsernameTaken));
                txtUserName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.UserForm_PasswordRequired,
                    ErrorCodesBLL.Validation.PasswordRequired));
                txtPassword.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtConfirm.Text))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.ChangePassword_RequiredFields,
                    ErrorCodesBLL.Validation.PasswordRequired));
                txtConfirm.Focus();
                return false;
            }

            if (!txtPassword.Text.Equals(txtConfirm.Text))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.ChangePassword_PasswordsMismatch,
                    ErrorCodesBLL.Validation.PasswordsMismatch));
                txtConfirm.Focus();
                return false;
            }

            return true;
        }

        private int FindAdminRoleId()
        {
            List<RoleBE> roles = _roleBLL.FindAll();
            RoleBE adminRole = roles.FirstOrDefault(r => r.Name == "Admin");
            return adminRole != null ? adminRole.Id : 0;
        }

        private UserBE MapUserFromUI()
        {
            return new UserBE
            {
                UserName = txtUserName.Text.Trim(),
                PasswordHash = EncryptionBLL.HashPassword(txtPassword.Text),
                IsActive = true,
                RetriesCount = 0,
                CreatedAt = DateTime.Now,
                LastUpdate = DateTime.Now,
                Language = cbLanguage.SelectedValue?.ToString() ?? CultureHelperBLL.DefaultLanguage,
                Theme = cbTheme.SelectedValue?.ToString() ?? ThemeHelper.DefaultTheme,
                RoleId = FindAdminRoleId()
            };
        }

        private void OpenApplication(UserBE user)
        {
            CultureHelperBLL.SetCulture(user.Language);
            AppPreferencesBLL.SavePreferences(user.Language, user.Theme);
            SessionManagerBLL.CreateSession(user);

            try
            {
                ServiceLocatorBLL.CreateActivityBLL().LogLogin(user.Id, user.UserName);
            }
            catch
            {
            }

            this.Hide();

            using (var mainForm = new MainForm(user))
            {
                mainForm.ShowDialog();
            }

            SessionManagerBLL.RemoveSession(user.Id);

            using (var loginForm = new LoginForm())
            {
                loginForm.ShowDialog();
            }

            this.Close();
        }
        #endregion

        #region Eventos
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            UserBE user = MapUserFromUI();

            if (user.RoleId == 0)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Auth_DbUnavailable,
                    ErrorCodesBLL.Database.Unavailable));
                return;
            }

            if (!_userBLL.Save(user))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.UserForm_UserNameTaken,
                    ErrorCodesBLL.Validation.UsernameTaken));
                txtUserName.Focus();
                return;
            }

            MessageBox.Show(Resources.Register_Success);
            OpenApplication(user);
        }
        #endregion
    }
}
