using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class EnrollmentManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IEnrollmentBLL _enrollmentBLL;
        #endregion

        #region Constructores
        public EnrollmentManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _enrollmentBLL = ServiceLocatorBLL.CreateEnrollmentBLL();
            ApplyResources();
            LoadEnrollments();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.EnrollmentManagementForm_Title;
            lblSearch.Text = Resources.EnrollmentManagementForm_SearchLabel;
            btnSearch.Text = Resources.EnrollmentManagementForm_SearchButton;
        }

        private string GetDayOfWeekName(int? dia)
        {
            if (!dia.HasValue) return "-";
            switch (dia.Value)
            {
                case 1: return Resources.Day_Monday;
                case 2: return Resources.Day_Tuesday;
                case 3: return Resources.Day_Wednesday;
                case 4: return Resources.Day_Thursday;
                case 5: return Resources.Day_Friday;
                case 6: return Resources.Day_Saturday;
                case 7: return Resources.Day_Sunday;
                default: return "-";
            }
        }

        private void LoadEnrollments()
        {
            dgvEnrollments.DataSource = null;
            var enrollments = _enrollmentBLL.FindAll();

            var inscripcionesGrid = enrollments.Select(i => new
            {
                Alumno = i.StudentName,
                Curso = i.CourseName,
                DiaSemana = GetDayOfWeekName(i.DayOfWeek),
                HoraInicio = i.StartTime.HasValue ? i.StartTime.Value.ToString(@"hh\:mm") : "-",
                HoraFin = i.EndTime.HasValue ? i.EndTime.Value.ToString(@"hh\:mm") : "-",
                Aula = i.ClassroomName ?? Resources.EnrollmentForm_NoAssignment,
                Fecha = i.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvEnrollments.DataSource = inscripcionesGrid;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvEnrollments.Columns.Count == 0) return;

            dgvEnrollments.Columns["Alumno"].HeaderText = Resources.EnrollmentManagementForm_ColStudent;
            dgvEnrollments.Columns["Curso"].HeaderText = Resources.EnrollmentManagementForm_ColCourse;
            dgvEnrollments.Columns["DiaSemana"].HeaderText = Resources.EnrollmentManagementForm_ColDay;
            dgvEnrollments.Columns["HoraInicio"].HeaderText = Resources.EnrollmentManagementForm_ColStartTime;
            dgvEnrollments.Columns["HoraFin"].HeaderText = Resources.EnrollmentManagementForm_ColEndTime;
            dgvEnrollments.Columns["Aula"].HeaderText = Resources.EnrollmentManagementForm_ColClassroom;
            dgvEnrollments.Columns["Fecha"].HeaderText = Resources.EnrollmentManagementForm_ColEnrollDate;

            dgvEnrollments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEnrollments.MultiSelect = false;
            dgvEnrollments.ReadOnly = true;
            dgvEnrollments.AllowUserToAddRows = false;
            dgvEnrollments.AllowUserToDeleteRows = false;
        }
        #endregion

        #region Eventos
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEnrollments();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadEnrollments();
                return;
            }

            dgvEnrollments.DataSource = null;
            var enrollments = _enrollmentBLL.FindAll();

            var inscripcionesFiltradas = enrollments
                .Where(i => i.StudentName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                         || i.CourseName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            var inscripcionesGrid = inscripcionesFiltradas.Select(i => new
            {
                Alumno = i.StudentName,
                Curso = i.CourseName,
                DiaSemana = GetDayOfWeekName(i.DayOfWeek),
                HoraInicio = i.StartTime.HasValue ? i.StartTime.Value.ToString(@"hh\:mm") : "-",
                HoraFin = i.EndTime.HasValue ? i.EndTime.Value.ToString(@"hh\:mm") : "-",
                Aula = i.ClassroomName ?? Resources.EnrollmentForm_NoAssignment,
                Fecha = i.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvEnrollments.DataSource = inscripcionesGrid;
            ConfigureGrid();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
        #endregion
    }
}
