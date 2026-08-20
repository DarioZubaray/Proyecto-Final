using System;
using System.Windows.Forms;

using BLL;
using BE;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        #region Propiedades
        private AuthBLL _authBLL;
        #endregion

        #region Inicializdores
        public LoginForm()
        {
            InitializeComponent();
            _authBLL = new AuthBLL();
        }
        #endregion

        #region Eventos
        private void button1_Click(object sender, EventArgs e)
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
                button1.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        #endregion
    }
}
