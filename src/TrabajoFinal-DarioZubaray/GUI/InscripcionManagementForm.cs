using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
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
            LoadInscripciones();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private string GetDiaSemanaNombre(int? dia)
        {
            if (!dia.HasValue) return "-";
            switch (dia.Value)
            {
                case 1: return "Lunes";
                case 2: return "Martes";
                case 3: return "Miércoles";
                case 4: return "Jueves";
                case 5: return "Viernes";
                case 6: return "Sábado";
                case 7: return "Domingo";
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
                Aula = i.AulaNombre ?? "Sin asignar",
                Fecha = i.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            }).ToList();

            dgvInscripciones.DataSource = inscripcionesGrid;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvInscripciones.Columns.Count == 0) return;

            dgvInscripciones.Columns["Alumno"].HeaderText = "Alumno";
            dgvInscripciones.Columns["Curso"].HeaderText = "Curso";
            dgvInscripciones.Columns["DiaSemana"].HeaderText = "Día";
            dgvInscripciones.Columns["HoraInicio"].HeaderText = "Hora Inicio";
            dgvInscripciones.Columns["HoraFin"].HeaderText = "Hora Fin";
            dgvInscripciones.Columns["Aula"].HeaderText = "Aula";
            dgvInscripciones.Columns["Fecha"].HeaderText = "Fecha Inscripción";

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
                Aula = i.AulaNombre ?? "Sin asignar",
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
