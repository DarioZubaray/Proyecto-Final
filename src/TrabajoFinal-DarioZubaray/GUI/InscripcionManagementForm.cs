using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class InscripcionManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IInscripcionBLL _inscripcionBLL;
        #endregion

        #region Constructores
        public InscripcionManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _inscripcionBLL = ServiceLocatorBLL.CreateInscripcionBLL();
            ApplyResources();
            LoadInscripciones();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.InscripcionManagementForm_Title;
            lblSearch.Text = Resources.InscripcionManagementForm_SearchLabel;
            btnSearch.Text = Resources.InscripcionManagementForm_SearchButton;
        }

        private string GetDiaSemanaNombre(int? dia)
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

        private void LoadInscripciones()
        {
            dgvInscripciones.DataSource = null;
            var inscripciones = _inscripcionBLL.FindAll();

            var inscripcionesGrid = inscripciones.Select(i => new
            {
                Alumno = i.AlumnoNombre,
                Curso = i.CursoNombre,
                DiaSemana = GetDiaSemanaNombre(i.DiaSemana),
                HoraInicio = i.HoraInicio.HasValue ? i.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = i.HoraFin.HasValue ? i.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Aula = i.AulaNombre ?? Resources.InscripcionForm_NoAssignment,
                Fecha = i.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvInscripciones.DataSource = inscripcionesGrid;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvInscripciones.Columns.Count == 0) return;

            dgvInscripciones.Columns["Alumno"].HeaderText = Resources.InscripcionManagementForm_ColStudent;
            dgvInscripciones.Columns["Curso"].HeaderText = Resources.InscripcionManagementForm_ColCourse;
            dgvInscripciones.Columns["DiaSemana"].HeaderText = Resources.InscripcionManagementForm_ColDay;
            dgvInscripciones.Columns["HoraInicio"].HeaderText = Resources.InscripcionManagementForm_ColStartTime;
            dgvInscripciones.Columns["HoraFin"].HeaderText = Resources.InscripcionManagementForm_ColEndTime;
            dgvInscripciones.Columns["Aula"].HeaderText = Resources.InscripcionManagementForm_ColClassroom;
            dgvInscripciones.Columns["Fecha"].HeaderText = Resources.InscripcionManagementForm_ColEnrollDate;

            dgvInscripciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInscripciones.MultiSelect = false;
            dgvInscripciones.ReadOnly = true;
            dgvInscripciones.AllowUserToAddRows = false;
            dgvInscripciones.AllowUserToDeleteRows = false;
        }
        #endregion

        #region Eventos
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInscripciones();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadInscripciones();
                return;
            }

            dgvInscripciones.DataSource = null;
            var inscripciones = _inscripcionBLL.FindAll();

            var inscripcionesFiltradas = inscripciones
                .Where(i => i.AlumnoNombre.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                         || i.CursoNombre.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            var inscripcionesGrid = inscripcionesFiltradas.Select(i => new
            {
                Alumno = i.AlumnoNombre,
                Curso = i.CursoNombre,
                DiaSemana = GetDiaSemanaNombre(i.DiaSemana),
                HoraInicio = i.HoraInicio.HasValue ? i.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = i.HoraFin.HasValue ? i.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Aula = i.AulaNombre ?? Resources.InscripcionForm_NoAssignment,
                Fecha = i.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvInscripciones.DataSource = inscripcionesGrid;
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
