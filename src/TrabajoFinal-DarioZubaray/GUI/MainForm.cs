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
            alumnosToolStripMenuItem.Text = Resources.Main_MenuStudents;
            académicoToolStripMenuItem.Text = Resources.Main_MenuAcademic;
            cursosToolStripMenuItem1.Text = Resources.Main_MenuCourses;
            inscripcionesToolStripMenuItem1.Text = Resources.Main_MenuEnrollments;
            asistenciaToolStripMenuItem.Text = Resources.Main_MenuAttendance;
            evaluacionesToolStripMenuItem.Text = Resources.Main_MenuEvaluations;
            reportesToolStripMenuItem.Text = Resources.Main_MenuReports;
            rendimientoToolStripMenuItem.Text = Resources.Main_MenuPerformance;
            informesToolStripMenuItem.Text = Resources.Main_MenuReportsCustom;
            aulasToolStripMenuItem.Text = Resources.Main_MenuClassrooms;
            integridadToolStripMenuItem.Text = Resources.Integrity_Title;
            respaldoToolStripMenuItem.Text = Resources.Main_MenuBackup;
            ticketsToolStripMenuItem.Text = Resources.Main_MenuTickets;
            soporteToolStripMenuItem.Text = Resources.Main_MenuSupport;
            ayudaToolStripMenuItem.Text = Resources.Main_MenuHelp;
            acercaDeToolStripMenuItem.Text = Resources.Main_MenuAbout;
            UpdateFooter();
        }

        #region Métodos
        private void ConfigureMenuVisibility()
        {
            bool canPreferences = EnsurePermission("FORM_PREFERENCES");
            bool canChangePassword = EnsurePermission("FORM_CHANGE_PASS");
            bool canViewHistory = EnsurePermission("FORM_ACTIVITY_HISTORY");
            bool canLogout = EnsurePermission("FORM_LOGOUT");
            bool canAbout = EnsurePermission("FORM_ABOUT");

            archivoToolStripMenuItem.Visible = canPreferences || canChangePassword || canViewHistory || canLogout;
            preferenciasToolStripMenuItem.Visible = canPreferences;
            cambiarContraseñaToolStripMenuItem.Visible = canChangePassword;
            historialActividadToolStripMenuItem.Visible = canViewHistory;
            cerrarSesiónToolStripMenuItem.Visible = canLogout;

            administraciónToolStripMenuItem.Visible = _session != null
                && (_session.HasPermission("FORM_USER_MGMT")
                    || _session.HasPermission("FORM_STUDENT_MGMT")
                    || _session.HasPermission("FORM_BACKUP_MGMT"));
            usuariosToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_USER_MGMT");
            rolesToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_ROLE_MGMT");
            alumnosToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_STUDENT_MGMT");
            aulasToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_CURSO_MGMT");

            integridadToolStripMenuItem.Visible = UserBLL.IsSysAdmin(_user);
            respaldoToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_BACKUP_MGMT");
            soporteToolStripMenuItem.Visible = _session != null
                && _session.HasPermission("FORM_TICKET_CREATE");

            bool canManageCourses = _session != null && _session.HasPermission("FORM_CURSO_MGMT");
            bool canViewEnrollments = _session != null && _session.HasPermission("FORM_INSCRIPCION_MGMT");
            bool canRegisterAttendance = _session != null && _session.HasPermission("FORM_ASISTENCIA_MGMT");
            bool canViewAttendance = _session != null && _session.HasPermission("FORM_ASISTENCIA_VIEW");
            bool canManageEvaluations = _session != null && _session.HasPermission("FORM_EVAL_MGMT");
            bool canViewReports = _session != null && _session.HasPermission("FORM_REPORTS");
            bool canManageReportDetail = _session != null && _session.HasPermission("FORM_REPORT_DETAIL");

            académicoToolStripMenuItem.Visible = canManageCourses || canViewEnrollments || canRegisterAttendance || canViewAttendance || canManageEvaluations;
            cursosToolStripMenuItem1.Visible = canManageCourses;
            inscripcionesToolStripMenuItem1.Visible = canViewEnrollments;
            asistenciaToolStripMenuItem.Visible = canRegisterAttendance || canViewAttendance;
            evaluacionesToolStripMenuItem.Visible = canManageEvaluations;
            reportesToolStripMenuItem.Visible = canViewReports || canManageReportDetail;
            rendimientoToolStripMenuItem.Visible = canViewReports;
            informesToolStripMenuItem.Visible = canManageReportDetail;

            ayudaToolStripMenuItem.Visible = canAbout;
            acercaDeToolStripMenuItem.Visible = canAbout;
        }

        private bool EnsurePermission(string permission)
        {
            return _session != null && _session.HasPermission(permission);
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
            if (!EnsurePermission("FORM_LOGOUT"))
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region MenuItem_Click
        private void historialActividadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_ACTIVITY_HISTORY"))
            {
                return;
            }

            ShowChildForm(() => new ActivityHistoryForm(_user));
        }

        private void preferenciasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_PREFERENCES"))
            {
                return;
            }

            ShowChildForm(() => new PreferencesForm(_user, this), "PreferencesForm");
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_USER_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new UserManagementForm(_user), "UserManagementForm");
        }

        private void rolesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_ROLE_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new RoleManagementForm(_user), "RoleManagementForm");
        }

        private void alumnosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_STUDENT_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new StudentManagementForm(_user), "StudentManagementForm");
        }

        private void aulasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_CURSO_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new ClassroomManagementForm(_user), "ClassroomManagementForm");
        }

        private void ticketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_TICKET_CREATE"))
            {
                return;
            }

            ShowChildForm(() => new TicketManagementForm(_user), "TicketManagementForm");
        }

        private void integridadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UserBLL.IsSysAdmin(_user))
            {
                return;
            }

            LogFormAccess("IntegrityAuditForm");

            using (var form = new IntegrityAuditForm(_user))
            {
                form.ShowDialog(this);
            }
        }

        private void cursosToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_CURSO_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new CourseManagementForm(_user), "CourseManagementForm");
        }

        private void inscripcionesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_INSCRIPCION_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new EnrollmentManagementForm(_user), "EnrollmentManagementForm");

        }

        private void asistenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EnsurePermission("FORM_ASISTENCIA_MGMT"))
            {
                ShowChildForm(() => new AttendanceForm(_user), "AttendanceForm");
            }
            else if (EnsurePermission("FORM_ASISTENCIA_VIEW"))
            {
                ShowChildForm(() => new AttendanceViewForm(_user), "AttendanceViewForm");
            }
        }

        private void evaluacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_EVAL_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new EvaluationForm(_user), "EvaluationForm");
        }

        private void rendimientoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_REPORTS"))
            {
                return;
            }

            ShowChildForm(() => new PerformanceForm(_user), "PerformanceForm");
        }

        private void informesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_REPORT_DETAIL"))
            {
                return;
            }

            ShowChildForm(() => new ReportForm(_user), "ReportForm");
        }

        private void respaldoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_BACKUP_MGMT"))
            {
                return;
            }

            ShowChildForm(() => new BackupForm(_user), "BackupForm");
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_CHANGE_PASS"))
            {
                return;
            }

            ShowChildForm(() => new ChangePasswordForm(_user), "ChangePasswordForm");
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!EnsurePermission("FORM_ABOUT"))
            {
                return;
            }

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
