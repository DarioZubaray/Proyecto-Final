using System;
using System.Windows.Forms;
using BE;
using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    public partial class MainForm : Form
    {
        private readonly UserBE _user;

        public MainForm(UserBE user)
        {
            InitializeComponent();
            _user = user;
            ApplyResources();
            this.FormClosing += MainForm_FormClosing;
        }

        public void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            archivoToolStripMenuItem.Text = Resources.Main_MenuFile;
            preferenciasToolStripMenuItem.Text = Resources.Main_MenuPreferences;
            cerrarSesiónToolStripMenuItem.Text = Resources.Main_MenuLogout;
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

        private void preferenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new PreferencesForm(_user, this)
            {
                MdiParent = this
            };
            form.Show();
        }
    }
}
