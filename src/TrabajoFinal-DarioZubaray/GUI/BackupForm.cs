using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace TrabajoFinal_DarioZubaray
{
    public partial class BackupForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IBackupBLL _backupBLL;
        #endregion

        #region Constructor
        public BackupForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _backupBLL = ServiceLocatorBLL.CreateBackupBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.BackupForm_Title;
            groupBox1.Text = Resources.BackupForm_Title;
            lblInfo.Text = Resources.BackupForm_InfoLabel;
            btnGenerate.Text = Resources.BackupForm_GenerateButton;
            btnRestore.Text = Resources.BackupForm_RestoreButton;
            btnRestoreScript.Text = Resources.BackupForm_RestoreScriptButton;
        }
        #endregion

        #region Eventos
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = Resources.BackupForm_Filter;
                dialog.DefaultExt = "bak";
                dialog.FileName = "DarioZubaray_TF_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    if (_backupBLL.CreateBackup(dialog.FileName))
                    {
                        MessageBox.Show(Resources.BackupForm_BackupGenerated, this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.BackupForm_BackupError + " " + ex.Message,
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Resources.BackupForm_Filter;
                dialog.DefaultExt = "bak";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    if (_backupBLL.RestoreBackup(dialog.FileName))
                    {
                        MessageBox.Show(Resources.BackupForm_Restored, this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.BackupForm_RestoreError + " " + ex.Message,
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRestoreScript_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Resources.BackupForm_ScriptFilter;
                dialog.DefaultExt = "sql";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    if (_backupBLL.RestoreFromScript(dialog.FileName))
                    {
                        MessageBox.Show(Resources.BackupForm_ScriptRestored, this.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Resources.BackupForm_RestoreScriptError + " " + ex.Message,
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
