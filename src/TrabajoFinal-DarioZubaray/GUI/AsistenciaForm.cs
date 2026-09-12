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
    public partial class AsistenciaForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IAsistenciaBLL _asistenciaBLL;
        private readonly ICursoBLL _cursoBLL;
        private readonly IInscripcionBLL _inscripcionBLL;
        private List<CursoBE> _cursosDelDocente;
        #endregion

        #region Constructores
        public AsistenciaForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _asistenciaBLL = ServiceLocatorBLL.CreateAsistenciaBLL();
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _inscripcionBLL = ServiceLocatorBLL.CreateInscripcionBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
            LoadCursos();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.AsistenciaForm_Title;
            lblCurso.Text = Resources.AsistenciaForm_CourseLabel;
            lblFecha.Text = Resources.AsistenciaForm_DateLabel;
            btnBuscar.Text = Resources.AsistenciaForm_SearchButton;
            btnGuardar.Text = Resources.AsistenciaForm_SaveButton;
        }

        private void LoadCursos()
        {
            _cursosDelDocente = _cursoBLL.FindByDocenteId(_currentUser.Id);

            cboCurso.DataSource = null;
            cboCurso.DataSource = _cursosDelDocente;
            cboCurso.DisplayMember = "Nombre";
            cboCurso.ValueMember = "Id";

            if (_cursosDelDocente.Count == 0)
            {
                btnBuscar.Enabled = false;
                btnGuardar.Enabled = false;
                MessageBox.Show(Resources.AsistenciaForm_NoCourses,
                    Resources.AsistenciaForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadAlumnos()
        {
            if (cboCurso.SelectedValue == null) return;

            int cursoId = (int)cboCurso.SelectedValue;
            DateTime fecha = dtpFecha.Value.Date;

            dgvAsistencia.DataSource = null;
            dgvAsistencia.Columns.Clear();

            List<InscripcionBE> inscripciones = _inscripcionBLL.FindAll()
                .Where(i => i.CursoId == cursoId && i.IsActive)
                .ToList();

            if (inscripciones.Count == 0)
            {
                MessageBox.Show(Resources.AsistenciaForm_NoStudents,
                    Resources.AsistenciaForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<AsistenciaBE> asistenciaExistente = _asistenciaBLL.FindByCursoIdAndFecha(cursoId, fecha);
            Dictionary<int, bool> mapaAsistencia = asistenciaExistente.ToDictionary(a => a.AlumnoId, a => a.Presente);

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("AlumnoId", typeof(int));
            dt.Columns.Add("Alumno", typeof(string));
            dt.Columns.Add("Presente", typeof(bool));

            foreach (var inscripcion in inscripciones)
            {
                bool presente = mapaAsistencia.ContainsKey(inscripcion.AlumnoId) && mapaAsistencia[inscripcion.AlumnoId];
                dt.Rows.Add(inscripcion.AlumnoId, inscripcion.AlumnoNombre, presente);
            }

            dgvAsistencia.DataSource = dt;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvAsistencia.Columns.Count == 0) return;

            dgvAsistencia.Columns["AlumnoId"].Visible = false;
            dgvAsistencia.Columns["Alumno"].HeaderText = Resources.AsistenciaForm_ColStudent;
            dgvAsistencia.Columns["Alumno"].ReadOnly = true;
            dgvAsistencia.Columns["Alumno"].Width = 400;

            dgvAsistencia.Columns["Presente"].HeaderText = Resources.AsistenciaForm_ColPresent;
            dgvAsistencia.Columns["Presente"].ReadOnly = false;
            dgvAsistencia.Columns["Presente"].Width = 100;
            dgvAsistencia.Columns["Presente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAsistencia.Columns["Presente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAsistencia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAsistencia.MultiSelect = false;
            dgvAsistencia.AllowUserToAddRows = false;
            dgvAsistencia.AllowUserToDeleteRows = false;
            dgvAsistencia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        #endregion

        #region Eventos
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cboCurso.SelectedValue == null)
            {
                MessageBox.Show(Resources.AsistenciaForm_SelectCourse,
                    Resources.AsistenciaForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cursoId = (int)cboCurso.SelectedValue;

            if (!_asistenciaBLL.EsDocenteDelCurso(cursoId, _currentUser.Id))
            {
                MessageBox.Show(Resources.AsistenciaForm_Unauthorized,
                    Resources.AsistenciaForm_ErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            LoadAlumnos();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboCurso.SelectedValue == null)
                {
                    MessageBox.Show(Resources.AsistenciaForm_SelectCourse,
                        Resources.AsistenciaForm_ValidationTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dgvAsistencia.Rows.Count == 0)
                {
                    MessageBox.Show(Resources.AsistenciaForm_NoStudents,
                        Resources.AsistenciaForm_ValidationTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int cursoId = (int)cboCurso.SelectedValue;
                DateTime fecha = dtpFecha.Value.Date;

                if (!_asistenciaBLL.EsDocenteDelCurso(cursoId, _currentUser.Id))
                {
                    MessageBox.Show(Resources.AsistenciaForm_Unauthorized,
                        Resources.AsistenciaForm_ErrorTitle,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                List<AsistenciaBE> registros = new List<AsistenciaBE>();

                foreach (DataGridViewRow row in dgvAsistencia.Rows)
                {
                    if (row.Cells["AlumnoId"].Value == null) continue;

                    int alumnoId = Convert.ToInt32(row.Cells["AlumnoId"].Value);
                    bool presente = Convert.ToBoolean(row.Cells["Presente"].Value);

                    registros.Add(new AsistenciaBE
                    {
                        CursoId = cursoId,
                        AlumnoId = alumnoId,
                        Fecha = fecha,
                        Presente = presente
                    });
                }

                _asistenciaBLL.RegistrarAsistenciaBulk(cursoId, fecha, registros);

                MessageBox.Show(Resources.AsistenciaForm_SaveSuccess,
                    Resources.AsistenciaForm_Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.AsistenciaForm_SaveError, ex.Message),
                    Resources.AsistenciaForm_ErrorTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
