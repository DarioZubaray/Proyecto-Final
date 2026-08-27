using System;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        #region Propiedades
        private IAuthBLL _authBLL;
        #endregion

        #region Constructor
        public LoginForm()
        {
            InitializeComponent();
            _authBLL = ServiceLocatorBLL.CreateAuthBLL();
            ApplyResources();
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
        }
        #endregion

        #region Eventos
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            var username = txtUser.Text;
            var password = txtPass.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return;
            }

            LoginResultBE result = _authBLL.Login(username, password);

            if (result.Success)
            {
                SessionManagerBLL.CreateSession(result.User);
                this.Hide();
                MainForm mainForm = new MainForm(result.User);
                DialogResult dialogResult = mainForm.ShowDialog();
                SessionManagerBLL.RemoveSession(result.User.Id);
                CultureHelperBLL.SetCulture(CultureHelperBLL.DefaultLanguage);
                ApplyResources();
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
