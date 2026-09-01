using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BE.Composite;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;

namespace TrabajoFinal_DarioZubaray
{
    public partial class MainForm : Form
    {
        #region Propiedades
        private readonly UserBE _user;
        private readonly SessionManagerBLL _session;
        #endregion

        #region Constructor
        public MainForm(UserBE user)
        {
            InitializeComponent();
            _user = user;
            _session = SessionManagerBLL.GetInstance(user.Id);
            ApplyResources();
            ApplyTheme();
            ConfigureMenuVisibility();
            UpdateFooter();
            this.FormClosing += MainForm_FormClosing;
        }
        #endregion

        public void ApplyTheme()
        {
            ThemeHelper.ApplyTheme(this, _user.Theme ?? ThemeHelper.DefaultTheme);
        }

        public void ApplyResources()
        {
            this.Text = Resources.Main_Title;
            archivoToolStripMenuItem.Text = Resources.Main_MenuFile;
            preferenciasToolStripMenuItem.Text = Resources.Main_MenuPreferences;
            cambiarContraseñaToolStripMenuItem.Text = Resources.Main_MenuChangePassword;
            historialActividadToolStripMenuItem.Text = Resources.Main_MenuActivityHistory;
            cerrarSesiónToolStripMenuItem.Text = Resources.Main_MenuLogout;
            administraciónToolStripMenuItem.Text = Resources.Main_MenuAdministration;
            usuariosToolStripMenuItem.Text = Resources.Main_MenuUsers;
            rolesToolStripMenuItem.Text = Resources.Main_MenuRoles;
            ayudaToolStripMenuItem.Text = Resources.Main_MenuHelp;
            acercaDeToolStripMenuItem.Text = Resources.Main_MenuAbout;
            UpdateFooter();
        }

        #region Métodos
        private void ConfigureMenuVisibility()
        {
            administraciónToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_USER_MGMT");
            rolesToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_ROLE_MGMT");
        }

        private void UpdateFooter()
        {
            lblUserInfo.Text = $" {Resources.Main_FooterUser} {_user.UserName} ";
            lblRoleInfo.Text = $" | {Resources.Main_FooterRole} {GetRoleNames()} ";
        }

        private string GetRoleNames()
        {
            if (_session?.RoleTree == null)
            {
                return "-";
            }

            var roleNames = CollectRoleNames(_session.RoleTree);
            return roleNames.Any() ? string.Join(", ", roleNames.Distinct()) : "-";
        }

        private List<string> CollectRoleNames(IRoleComponentBE component)
        {
            var names = new List<string>();

            if (component is RoleCompositeBE composite)
            {
                names.Add(composite.Name);
                foreach (var child in composite.GetChildren())
                {
                    names.AddRange(CollectRoleNames(child));
                }
            }

            return names;
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
        private void historialActividadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ActivityHistoryForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void preferenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("PreferencesForm");
            var form = new PreferencesForm(_user, this)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("UserManagementForm");
            var form = new UserManagementForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("RoleManagementForm");
            var form = new RoleManagementForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("ChangePasswordForm");
            var form = new ChangePasswordForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("AboutForm");
            var form = new AboutForm(_user)
            {
                MdiParent = this
            };
            form.Show();
        }

        private void LogFormAccess(string formName)
        {
            try
            {
                var activityBLL = ServiceLocatorBLL.CreateActivityBLL();
                activityBLL.LogFormAccess(_user.Id, formName);
            }
            catch
            {
                // Loguear el acceso no debe impedir abrir el formulario.
            }
        }
        #endregion
    }
}
