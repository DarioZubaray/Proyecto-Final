using System;
using System.Windows.Forms;
using BE;
using BE.Properties;
using BLL;

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
        }

        private void ApplyResources()
        {
            this.Text = Resources.ChangePassword_Title;
            groupBox1.Text = Resources.ChangePassword_GroupBox;
            lblCurrentPassword.Text = Resources.ChangePassword_CurrentLabel;
            lblNewPassword.Text = Resources.ChangePassword_NewLabel;
            lblConfirmPassword.Text = Resources.ChangePassword_ConfirmLabel;
            btnChange.Text = Resources.ChangePassword_ChangeButton;
            btnCancel.Text = Resources.ChangePassword_CancelButton;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCurrentPassword.Text)
                || string.IsNullOrEmpty(txtNewPassword.Text)
                || string.IsNullOrEmpty(txtConfirmPassword.Text))
            {
                MessageBox.Show(Resources.ChangePassword_RequiredFields);
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show(Resources.ChangePassword_PasswordsMismatch);
                txtNewPassword.Focus();
                return;
            }

            bool changed = _userBLL.ChangePassword(
                _user.Id, txtCurrentPassword.Text, txtNewPassword.Text);

            if (!changed)
            {
                MessageBox.Show(Resources.ChangePassword_InvalidCurrent);
                txtCurrentPassword.Focus();
                return;
            }

            MessageBox.Show(Resources.ChangePassword_Success);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
