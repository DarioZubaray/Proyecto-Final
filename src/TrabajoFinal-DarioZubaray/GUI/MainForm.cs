using System;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class MainForm : Form
    {
        #region Propiedades
        private readonly UserBE _user;
        private readonly SessionManager _session;
        #endregion

        #region Constructor
        public MainForm(UserBE user)
        {
            InitializeComponent();
            _user = user;
            _session = SessionManager.GetInstance(user.Id);
            ApplyResources();
            ConfigureMenuVisibility();
            this.FormClosing += MainForm_FormClosing;
        }
        #endregion

        public void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            archivoToolStripMenuItem.Text = Resources.Main_MenuFile;
            preferenciasToolStripMenuItem.Text = Resources.Main_MenuPreferences;
            cambiarContraseñaToolStripMenuItem.Text = Resources.Main_MenuChangePassword;
            cerrarSesiónToolStripMenuItem.Text = Resources.Main_MenuLogout;
            administraciónToolStripMenuItem.Text = Resources.Main_MenuAdministration;
            usuariosToolStripMenuItem.Text = Resources.Main_MenuUsers;
        }

        #region Métodos
        private void ConfigureMenuVisibility()
        {
            administraciónToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_USER_MGMT");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void cerrarSesiónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region MenuItem_Click
        private void preferenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new PreferencesForm(_user, this)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new UserManagementForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ChangePasswordForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }
        #endregion
    }
}
