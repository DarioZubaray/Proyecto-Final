using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class InscripcionForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICursoBLL _cursoBLL;
        private readonly IInscripcionBLL _inscripcionBLL;
        #endregion

        #region Constructor
        public InscripcionForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _inscripcionBLL = ServiceLocatorBLL.CreateInscripcionBLL();
            LoadData();
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

        private void LoadData()
        {
            LoadCursosDisponibles();
            LoadMisInscripciones();
        }

        private void LoadCursosDisponibles()
        {
            var todosCursos = _cursoBLL.FindAll();
            var misInscripciones = _inscripcionBLL.FindByAlumnoId(_currentUser.Id);
            var cursosInscriptosIds = misInscripciones.Select(i => i.CursoId).ToHashSet();

            var cursosDisponibles = todosCursos
                .Where(c => !cursosInscriptosIds.Contains(c.Id))
                .ToList();

            var cursosGrid = cursosDisponibles.Select(c => new
            {
                c.Id,
                c.Nombre,
                Aula = c.AulaNombre ?? "Sin asignar",
                DiaSemana = GetDiaSemanaNombre(c.DiaSemana),
                HoraInicio = c.HoraInicio.HasValue ? c.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = c.HoraFin.HasValue ? c.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Docentes = string.Join(", ", c.Docentes.Select(d => d.UserName))
            }).ToList();

            dgvCursosDisponibles.DataSource = cursosGrid;
            ConfigureCursosGrid();
        }

        private void LoadMisInscripciones()
        {
            var inscripciones = _inscripcionBLL.FindByAlumnoId(_currentUser.Id);

            var inscripcionesGrid = inscripciones.Select(i => new
            {
                i.CursoId,
                Curso = i.CursoNombre,
                DiaSemana = GetDiaSemanaNombre(i.DiaSemana),
                HoraInicio = i.HoraInicio.HasValue ? i.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = i.HoraFin.HasValue ? i.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Aula = i.AulaNombre ?? "Sin asignar"
            }).ToList();

            dgvMisInscripciones.DataSource = inscripcionesGrid;
            ConfigureInscripcionesGrid();
        }

        private void ConfigureCursosGrid()
        {
            if (dgvCursosDisponibles.Columns.Count == 0) return;

            dgvCursosDisponibles.Columns["Id"].HeaderText = "ID";
            dgvCursosDisponibles.Columns["Nombre"].HeaderText = "Curso";
            dgvCursosDisponibles.Columns["Aula"].HeaderText = "Aula";
            dgvCursosDisponibles.Columns["DiaSemana"].HeaderText = "Día";
            dgvCursosDisponibles.Columns["HoraInicio"].HeaderText = "Hora Inicio";
            dgvCursosDisponibles.Columns["HoraFin"].HeaderText = "Hora Fin";
            dgvCursosDisponibles.Columns["Docentes"].HeaderText = "Docentes";

            dgvCursosDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCursosDisponibles.MultiSelect = false;
            dgvCursosDisponibles.ReadOnly = true;
            dgvCursosDisponibles.AllowUserToAddRows = false;
            dgvCursosDisponibles.AllowUserToDeleteRows = false;
        }

        private void ConfigureInscripcionesGrid()
        {
            if (dgvMisInscripciones.Columns.Count == 0) return;

            dgvMisInscripciones.Columns["CursoId"].HeaderText = "ID";
            dgvMisInscripciones.Columns["Curso"].HeaderText = "Curso";
            dgvMisInscripciones.Columns["DiaSemana"].HeaderText = "Día";
            dgvMisInscripciones.Columns["HoraInicio"].HeaderText = "Hora Inicio";
            dgvMisInscripciones.Columns["HoraFin"].HeaderText = "Hora Fin";
            dgvMisInscripciones.Columns["Aula"].HeaderText = "Aula";

            dgvMisInscripciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMisInscripciones.MultiSelect = false;
            dgvMisInscripciones.ReadOnly = true;
            dgvMisInscripciones.AllowUserToAddRows = false;
            dgvMisInscripciones.AllowUserToDeleteRows = false;
        }
        #endregion

        #region Eventos
        private void btnInscribirse_Click(object sender, EventArgs e)
        {
            if (dgvCursosDisponibles.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un curso para inscribirse.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cursoId = Convert.ToInt32(dgvCursosDisponibles.CurrentRow.Cells["Id"].Value);
            string cursoNombre = dgvCursosDisponibles.CurrentRow.Cells["Nombre"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"¿Desea inscribirse en el curso '{cursoNombre}'?",
                "Confirmar inscripción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _inscripcionBLL.Inscribir(cursoId, _currentUser.Id);
                    MessageBox.Show("Inscripción realizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al inscribirse: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDesinscribirse_Click(object sender, EventArgs e)
        {
            if (dgvMisInscripciones.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una inscripción para desuscribirse.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cursoId = Convert.ToInt32(dgvMisInscripciones.CurrentRow.Cells["CursoId"].Value);
            string cursoNombre = dgvMisInscripciones.CurrentRow.Cells["Curso"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"¿Desea desuscribirse del curso '{cursoNombre}'?",
                "Confirmar desuscripción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _inscripcionBLL.Desinscribir(cursoId, _currentUser.Id);
                    MessageBox.Show("Desuscripción realizada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al desuscribirse: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
