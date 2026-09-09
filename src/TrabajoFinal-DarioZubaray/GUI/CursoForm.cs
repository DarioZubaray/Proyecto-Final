using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class CursoForm : Form
    {
        #region Propiedades
        private readonly ICursoBLL _cursoBLL;
        private readonly IAulaBLL _aulaBLL;
        private readonly IUserBLL _userBLL;
        private readonly CursoBE _curso;
        private readonly bool _isNewCurso;
        private List<UserBE> _docentesDisponibles;
        private List<UserBE> _docentesAsignados;
        #endregion

        #region Constructor
        public CursoForm(string theme)
        {
            InitializeComponent();
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCurso = true;
            _curso = new CursoBE();
            _docentesDisponibles = new List<UserBE>();
            _docentesAsignados = new List<UserBE>();
            LoadAulas();
            LoadDocentes();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public CursoForm(CursoBE curso, string theme)
        {
            InitializeComponent();
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCurso = false;
            _curso = curso;
            _docentesAsignados = new List<UserBE>(curso.Docentes ?? new List<UserBE>());
            LoadAulas();
            LoadDocentes();
            LoadCursoData();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtNombre.Text.Trim()))
            {
                MessageBox.Show("El nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (dtpFechaInicio.Value >= dtpFechaFin.Value)
            {
                MessageBox.Show("La fecha de inicio debe ser anterior a la fecha de fin.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaInicio.Focus();
                return false;
            }

            if (_docentesAsignados.Count == 0)
            {
                MessageBox.Show("Debe asignar al menos un docente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void MapCursoFromUI()
        {
            _curso.Nombre = txtNombre.Text.Trim();
            _curso.Descripcion = txtDescripcion.Text.Trim();
            _curso.FechaInicio = dtpFechaInicio.Value;
            _curso.FechaFin = dtpFechaFin.Value;
            _curso.AulaId = cbAula.SelectedValue != null ? (int?)cbAula.SelectedValue : null;
            _curso.IsActive = true;

            if (_isNewCurso)
            {
                _curso.CreatedAt = DateTime.Now;
            }

            _curso.LastUpdate = DateTime.Now;
        }

        private bool SaveCurso()
        {
            bool result = _cursoBLL.Save(_curso);

            if (result)
            {
                List<int> docenteIds = _docentesAsignados.Select(d => d.Id).ToList();
                _cursoBLL.SaveDocentes(_curso.Id, docenteIds);
            }

            return result;
        }

        private void LoadAulas()
        {
            List<AulaBE> aulas = _aulaBLL.FindAll();

            var aulaItems = new List<object>();
            aulaItems.Add(new { Id = (int?)null, Nombre = "-- Sin asignar --" });
            aulaItems.AddRange(aulas.Select(a => new { Id = (int?)a.Id, a.Nombre }));

            cbAula.DataSource = aulaItems;
            cbAula.DisplayMember = "Nombre";
            cbAula.ValueMember = "Id";
            cbAula.SelectedIndex = 0;
        }

        private void LoadDocentes()
        {
            List<UserBE> allUsers = _userBLL.FindAll();
            _docentesDisponibles = allUsers.Where(u => u.IsActive && u.RoleId == 2).ToList();

            RefreshDocentesLists();
        }

        private void RefreshDocentesLists()
        {
            var docentesAsignadosIds = _docentesAsignados.Select(d => d.Id).ToHashSet();
            var disponibles = _docentesDisponibles.Where(d => !docentesAsignadosIds.Contains(d.Id)).ToList();

            lstDocentesDisponibles.DataSource = null;
            lstDocentesDisponibles.DataSource = disponibles;
            lstDocentesDisponibles.DisplayMember = "UserName";
            lstDocentesDisponibles.ValueMember = "Id";

            lstDocentesAsignados.DataSource = null;
            lstDocentesAsignados.DataSource = _docentesAsignados;
            lstDocentesAsignados.DisplayMember = "UserName";
            lstDocentesAsignados.ValueMember = "Id";
        }

        private void LoadCursoData()
        {
            txtNombre.Text = _curso.Nombre;
            txtDescripcion.Text = _curso.Descripcion;
            dtpFechaInicio.Value = _curso.FechaInicio;
            dtpFechaFin.Value = _curso.FechaFin;

            if (_curso.AulaId.HasValue)
            {
                cbAula.SelectedValue = _curso.AulaId;
            }
            else
            {
                cbAula.SelectedIndex = 0;
            }
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            MapCursoFromUI();

            try
            {
                if (SaveCurso())
                {
                    MessageBox.Show(_isNewCurso ? "Curso creado exitosamente." : "Curso modificado exitosamente.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el curso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAddDocente_Click(object sender, EventArgs e)
        {
            if (lstDocentesDisponibles.SelectedItem == null)
            {
                return;
            }

            var docente = lstDocentesDisponibles.SelectedItem as UserBE;
            if (docente != null)
            {
                _docentesAsignados.Add(docente);
                RefreshDocentesLists();
            }
        }

        private void btnRemoveDocente_Click(object sender, EventArgs e)
        {
            if (lstDocentesAsignados.SelectedItem == null)
            {
                return;
            }

            var docente = lstDocentesAsignados.SelectedItem as UserBE;
            if (docente != null)
            {
                _docentesAsignados.Remove(docente);
                RefreshDocentesLists();
            }
        }
        #endregion
    }
}
