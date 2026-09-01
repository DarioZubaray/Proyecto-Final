using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using BE;
using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    public partial class AboutForm : Form
    {
        #region Constantes
        private const string RepositoryUrl = "https://github.com/DarioZubaray/Proyecto-Final";
        #endregion

        #region Propiedades
        private readonly UserBE _user;
        #endregion

        #region Constructor
        public AboutForm(UserBE user)
        {
            InitializeComponent();
            _user = user;
            ApplyResources();
            ApplyTheme();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.About_Title;
            lblAppName.Text = Resources.About_AppName;
            lblVersion.Text = $"{Resources.About_Version} {GetVersion()}";
            lblCopyright.Text = Resources.About_Copyright;
            lblRepositoryLabel.Text = Resources.About_RepositoryLabel;
            btnClose.Text = Resources.About_CloseButton;
            linkRepository.Text = RepositoryUrl;
            linkRepository.Links.Clear();
            linkRepository.Links.Add(0, linkRepository.Text.Length, RepositoryUrl);
        }

        private void ApplyTheme()
        {
            ThemeHelper.ApplyTheme(this, _user?.Theme ?? ThemeHelper.DefaultTheme);
        }

        private string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString(3) : "1.0";
        }
        #endregion

        #region Eventos
        private void linkRepository_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = RepositoryUrl,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Abrir el navegador no debe romper el formulario About.
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
