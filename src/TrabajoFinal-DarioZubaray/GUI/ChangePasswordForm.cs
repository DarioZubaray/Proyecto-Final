using System;
using System.Windows.Forms;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ChangePasswordForm : Form
    {
        private readonly UserBE _user;
        private readonly IUserBLL _userBLL;

        public ChangePasswordForm(UserBE user)
        {
            InitializeComponent();
            _user = user;
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _user.Theme ?? ThemeHelper.DefaultTheme);
        }

        private void ApplyResources()
        {
            this.Text = Resources.ChangePassword_Title;
            groupBox1.Text = Resources.ChangePassword_GroupBox;
            lblCurrentPassword.Text = Resources.ChangePassword_CurrentLabel;
            lblNewPassword.Text = Resources.ChangePassword_NewLabel;
            lblConfirmPassword.Text = Resources.ChangePassword_ConfirmLabel;
            btnChange.Text = Resources.ChangePassword_ChangeButton;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCurrentPassword.Text)
                || string.IsNullOrEmpty(txtNewPassword.Text)
                || string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.ChangePassword_RequiredFields,
                    ErrorCodesBLL.Validation.PasswordRequired));
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.ChangePassword_PasswordsMismatch,
                    ErrorCodesBLL.Validation.PasswordsMismatch));
                txtNewPassword.Focus();
                return;
            }

            bool changed = _userBLL.ChangePassword(
                _user.Id, txtCurrentPassword.Text, txtNewPassword.Text);

            if (!changed)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.ChangePassword_InvalidCurrent,
                    ErrorCodesBLL.Business.InvalidCurrentPassword));
                txtCurrentPassword.Focus();
                return;
            }

            MessageBox.Show(Resources.ChangePassword_Success);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
