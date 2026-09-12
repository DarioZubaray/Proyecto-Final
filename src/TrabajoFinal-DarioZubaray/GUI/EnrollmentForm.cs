using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class EnrollmentForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _cursoBLL;
        private readonly IEnrollmentBLL _inscripcionBLL;
        #endregion

        #region Constructor
        public EnrollmentForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _inscripcionBLL = ServiceLocatorBLL.CreateInscripcionBLL();
            ApplyResources();
            LoadData();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.InscripcionForm_Title;
            lblCursosDisponibles.Text = Resources.InscripcionForm_AvailableCoursesLabel;
            lblMisInscripciones.Text = Resources.InscripcionForm_MyEnrollmentsLabel;
            btnInscribirse.Text = Resources.InscripcionForm_EnrollButton;
            btnDesinscribirse.Text = Resources.InscripcionForm_UnenrollButton;
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
                Aula = c.AulaNombre ?? Resources.InscripcionForm_NoAssignment,
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
                Aula = i.AulaNombre ?? Resources.InscripcionForm_NoAssignment
            }).ToList();

            dgvMisInscripciones.DataSource = inscripcionesGrid;
            ConfigureInscripcionesGrid();
        }

        private void ConfigureCursosGrid()
        {
            if (dgvCursosDisponibles.Columns.Count == 0) return;

            dgvCursosDisponibles.Columns["Id"].HeaderText = Resources.InscripcionForm_ColId;
            dgvCursosDisponibles.Columns["Nombre"].HeaderText = Resources.InscripcionForm_ColCourse;
            dgvCursosDisponibles.Columns["Aula"].HeaderText = Resources.InscripcionForm_ColClassroom;
            dgvCursosDisponibles.Columns["DiaSemana"].HeaderText = Resources.InscripcionForm_ColDay;
            dgvCursosDisponibles.Columns["HoraInicio"].HeaderText = Resources.InscripcionForm_ColStartTime;
            dgvCursosDisponibles.Columns["HoraFin"].HeaderText = Resources.InscripcionForm_ColEndTime;
            dgvCursosDisponibles.Columns["Docentes"].HeaderText = Resources.InscripcionForm_ColTeachers;

            dgvCursosDisponibles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCursosDisponibles.MultiSelect = false;
            dgvCursosDisponibles.ReadOnly = true;
            dgvCursosDisponibles.AllowUserToAddRows = false;
            dgvCursosDisponibles.AllowUserToDeleteRows = false;
        }

        private void ConfigureInscripcionesGrid()
        {
            if (dgvMisInscripciones.Columns.Count == 0) return;

            dgvMisInscripciones.Columns["CursoId"].HeaderText = Resources.InscripcionForm_ColId;
            dgvMisInscripciones.Columns["Curso"].HeaderText = Resources.InscripcionForm_ColCourse;
            dgvMisInscripciones.Columns["DiaSemana"].HeaderText = Resources.InscripcionForm_ColDay;
            dgvMisInscripciones.Columns["HoraInicio"].HeaderText = Resources.InscripcionForm_ColStartTime;
            dgvMisInscripciones.Columns["HoraFin"].HeaderText = Resources.InscripcionForm_ColEndTime;
            dgvMisInscripciones.Columns["Aula"].HeaderText = Resources.InscripcionForm_ColClassroom;

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
                MessageBox.Show(Resources.InscripcionForm_SelectCourseToEnroll, Resources.InscripcionForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cursoId = Convert.ToInt32(dgvCursosDisponibles.CurrentRow.Cells["Id"].Value);
            string cursoNombre = dgvCursosDisponibles.CurrentRow.Cells["Nombre"].Value.ToString();

            string confirmMessage = string.Format(Resources.InscripcionForm_ConfirmEnrollMessage, cursoNombre);
            DialogResult result = MessageBox.Show(
                confirmMessage,
                Resources.InscripcionForm_ConfirmEnrollTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _inscripcionBLL.Inscribir(cursoId, _currentUser.Id);
                    MessageBox.Show(Resources.InscripcionForm_EnrollSuccess, Resources.InscripcionForm_SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, Resources.InscripcionForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format(Resources.InscripcionForm_EnrollError, ex.Message);
                    MessageBox.Show(errorMsg, Resources.InscripcionForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDesinscribirse_Click(object sender, EventArgs e)
        {
            if (dgvMisInscripciones.CurrentRow == null)
            {
                MessageBox.Show(Resources.InscripcionForm_SelectEnrollmentToUnenroll, Resources.InscripcionForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int cursoId = Convert.ToInt32(dgvMisInscripciones.CurrentRow.Cells["CursoId"].Value);
            string cursoNombre = dgvMisInscripciones.CurrentRow.Cells["Curso"].Value.ToString();

            string confirmMessage = string.Format(Resources.InscripcionForm_ConfirmUnenrollMessage, cursoNombre);
            DialogResult result = MessageBox.Show(
                confirmMessage,
                Resources.InscripcionForm_ConfirmUnenrollTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _inscripcionBLL.Desinscribir(cursoId, _currentUser.Id);
                    MessageBox.Show(Resources.InscripcionForm_UnenrollSuccess, Resources.InscripcionForm_SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format(Resources.InscripcionForm_UnenrollError, ex.Message);
                    MessageBox.Show(errorMsg, Resources.InscripcionForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
