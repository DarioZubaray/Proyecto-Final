using System;
using System.Windows.Forms;

using BLL;
using BE;
using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        #region Fields
        private IAuthBLL _authBLL;
        #endregion

        #region Constructor
        public LoginForm()
        {
            InitializeComponent();
            _authBLL = ServiceLocator.CreateAuthBLL();
            ApplyResources();
        }
        #endregion

        #region Private Methods
        private void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            groupBox1.Text = Resources.Login_Title;
            lblUser.Text = Resources.Login_UsernameLabel;
            lblPass.Text = Resources.Login_PasswordLabel;
            btnLogin.Text = Resources.Login_Button;
            lblDeveloper.Text = Resources.Login_Developer;
            lblMessage.Text = Resources.Login_MessageInvalid;
        }
        #endregion

        #region Events
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            var username = txtUser.Text;
            var password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            LoginResult result = _authBLL.Login(username, password);

            if (result.Success)
            {
                this.Hide();
                MainForm mainForm = new MainForm();
                DialogResult dialogResult = mainForm.ShowDialog();
                this.Show();
                txtUser.Text = "";
                txtUser.Focus();
                txtPass.Text = "";
                lblMessage.Visible = false;
            }
            else
            {
                lblMessage.Text = result.Message;
                lblMessage.Visible = true;
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
    }
}
