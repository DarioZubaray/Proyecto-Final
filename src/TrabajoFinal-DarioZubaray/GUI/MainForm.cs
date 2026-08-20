using System;
using System.Windows.Forms;
using TrabajoFinal_DarioZubaray.Properties;

namespace TrabajoFinal_DarioZubaray
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ApplyResources();
            this.FormClosing += MainForm_FormClosing;
        }

        private void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            archivoToolStripMenuItem.Text = Resources.Main_MenuFile;
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
    }
}
