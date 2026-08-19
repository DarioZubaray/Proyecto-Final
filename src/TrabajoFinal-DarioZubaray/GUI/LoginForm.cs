using System;
using System.Windows.Forms;

namespace TrabajoFinal_DarioZubaray
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            var user = txtUser.Text;
            var pass = txtPass.Text;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                return;
            }

            if (user == "admin" && pass == "admin")
            {
                this.Hide();
                MainForm mainForm = new MainForm();
                DialogResult result = mainForm.ShowDialog();
                this.Show();
                txtUser.Text = "";
                txtPass.Text = "";
                lblMessage.Visible = false;
            }
            else
            {
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
    }
}
