using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Composite;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Services;

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
            académicoToolStripMenuItem.Text = Resources.Main_MenuAcademic;
            cursosToolStripMenuItem.Text = Resources.Main_MenuCourses;
            inscripcionesToolStripMenuItem.Text = Resources.Main_MenuEnrollments;
            asistenciaToolStripMenuItem.Text = Resources.Main_MenuAttendance;
            aulasToolStripMenuItem.Text = Resources.Main_MenuClassrooms;
            integridadToolStripMenuItem.Text = Resources.Integrity_Title;
            ticketsToolStripMenuItem.Text = Resources.Main_MenuTickets;
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
            aulasToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_CURSO_MGMT");

            integridadToolStripMenuItem.Visible = UserBLL.IsSysAdmin(_user);
            ticketsToolStripMenuItem.Visible = _session != null;

            bool canManageCourses = _session != null && _session.HasPermission("FORM_CURSO_MGMT");
            bool canViewEnrollments = _session != null && _session.HasPermission("FORM_INSCRIPCION_MGMT");
            bool canRegisterAttendance = _session != null && _session.HasPermission("FORM_ASISTENCIA_MGMT");
            bool canViewAttendance = _session != null && _session.HasPermission("FORM_ASISTENCIA_VIEW");

            académicoToolStripMenuItem.Visible = canManageCourses || canViewEnrollments || canRegisterAttendance || canViewAttendance;
            cursosToolStripMenuItem.Visible = canManageCourses;
            inscripcionesToolStripMenuItem.Visible = canViewEnrollments;
            asistenciaToolStripMenuItem.Visible = canRegisterAttendance || canViewAttendance;
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

        private List<string> CollectRoleNames(RoleComponentBE component)
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
            ShowChildForm(() => new ActivityHistoryForm(_user));
        }

        private void preferenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new PreferencesForm(_user, this), "PreferencesForm");
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new UserManagementForm(_user), "UserManagementForm");
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new RoleManagementForm(_user), "RoleManagementForm");
        }

        private void aulasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new ClassroomManagementForm(_user), "ClassroomManagementForm");
        }

        private void ticketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new TicketManagementForm(_user), "TicketManagementForm");
        }

        private void integridadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogFormAccess("IntegrityAuditForm");

            using (var form = new IntegrityAuditForm(_user))
            {
                form.ShowDialog(this);
            }
        }

        private void cursosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new CourseManagementForm(_user), "CourseManagementForm");
        }

        private void inscripcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_user.RoleId == 3)
            {
                ShowChildForm(() => new EnrollmentForm(_user), "EnrollmentForm");
            }
            else
            {
                ShowChildForm(() => new EnrollmentManagementForm(_user), "EnrollmentManagementForm");
            }
        }

        private void asistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_session.HasPermission("FORM_ASISTENCIA_MGMT"))
            {
                ShowChildForm(() => new AttendanceForm(_user), "AttendanceForm");
            }
            else if (_session.HasPermission("FORM_ASISTENCIA_VIEW"))
            {
                ShowChildForm(() => new AttendanceViewForm(_user), "AttendanceViewForm");
            }
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new ChangePasswordForm(_user), "ChangePasswordForm");
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(() => new AboutForm(_user), "AboutForm");
        }

        private void ShowChildForm<TForm>(Func<TForm> factory, string logName = null) where TForm : Form
        {
            var existing = MdiChildren.OfType<TForm>().FirstOrDefault();

            if (existing != null)
            {
                FocusChildForm(existing);
                return;
            }

            if (logName != null)
            {
                LogFormAccess(logName);
            }

            var form = factory();
            form.MdiParent = this;
            form.Show();
        }

        private void FocusChildForm(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                form.WindowState = FormWindowState.Normal;
            }

            form.BringToFront();
        }

        private void LogFormAccess(string formName)
        {
            try
            {
                var activityBLL = ServiceLocatorBLL.CreateActivityBLL();
                activityBLL.LogFormAccess(_user.Id, formName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception LogFormAccess", e.Message);
            }
        }
        #endregion
    }
}
