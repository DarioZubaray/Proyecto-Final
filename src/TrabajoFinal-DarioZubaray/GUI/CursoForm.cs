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
        private readonly UserBE _currentUser;
        private List<UserBE> _docentesDisponibles;
        private List<UserBE> _docentesAsignados;
        #endregion

        #region Constructor
        public CursoForm(string theme, UserBE user = null)
        {
            InitializeComponent();
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCurso = true;
            _curso = new CursoBE();
            _currentUser = user;
            _docentesDisponibles = new List<UserBE>();
            _docentesAsignados = new List<UserBE>();
            LoadAulas();
            LoadDocentes();
            ConfigureButtons();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public CursoForm(CursoBE curso, string theme, UserBE user = null)
        {
            InitializeComponent();
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCurso = false;
            _curso = curso;
            _currentUser = user;
            _docentesAsignados = new List<UserBE>(curso.Docentes ?? new List<UserBE>());
            LoadAulas();
            LoadDocentes();
            LoadCursoData();
            ConfigureButtons();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private bool ParseHorario(MaskedTextBox masked, out TimeSpan hora)
        {
            hora = TimeSpan.Zero;

            if (!masked.MaskFull)
            {
                MessageBox.Show("Debe completar el horario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            string texto = masked.Text.Replace("_", "").Trim();
            string[] partes = texto.Split(':');

            if (partes.Length != 2
                || !int.TryParse(partes[0], out int horas)
                || !int.TryParse(partes[1], out int minutos))
            {
                MessageBox.Show("Formato de horario inválido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            if (horas < 0 || horas > 23)
            {
                MessageBox.Show("Las horas deben estar entre 00 y 23.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            if (minutos != 0 && minutos != 15 && minutos != 30 && minutos != 45)
            {
                MessageBox.Show("Los minutos deben ser 00, 15, 30 o 45.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            hora = new TimeSpan(horas, minutos, 0);
            return true;
        }

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

            if (!ParseHorario(mtbHoraInicio, out TimeSpan horaInicio))
                return false;

            if (!ParseHorario(mtbHoraFin, out TimeSpan horaFin))
                return false;

            if (horaInicio >= horaFin)
            {
                MessageBox.Show("La hora de inicio debe ser anterior a la hora de fin.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtbHoraInicio.Focus();
                return false;
            }

            if (cbAula.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un aula.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbAula.Focus();
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
            _curso.AulaId = (int)cbAula.SelectedValue;
            _curso.DiaSemana = (int)_curso.FechaInicio.DayOfWeek == 0 ? 7 : (int)_curso.FechaInicio.DayOfWeek;

            ParseHorario(mtbHoraInicio, out TimeSpan hInicio);
            ParseHorario(mtbHoraFin, out TimeSpan hFin);
            _curso.HoraInicio = hInicio;
            _curso.HoraFin = hFin;

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

        private bool ValidateDocentesTraslape()
        {
            if (_docentesAsignados.Count == 0)
            {
                return false;
            }

            List<int> docenteIds = _docentesAsignados.Select(d => d.Id).ToList();
            return _cursoBLL.ValidarTraslapeDocentes(_curso.Id, _curso.DiaSemana, _curso.HoraInicio, _curso.HoraFin, docenteIds);
        }

        private void LoadAulas()
        {
            List<AulaBE> aulas = _aulaBLL.FindAll();

            cbAula.DisplayMember = "Nombre";
            cbAula.ValueMember = "Id";
            cbAula.DataSource = aulas;
            if (cbAula.Items.Count > 0)
            {
                cbAula.SelectedIndex = 0;
            }
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

            cbAula.SelectedValue = _curso.AulaId;

            if (_curso.HoraInicio.HasValue)
            {
                mtbHoraInicio.Text = _curso.HoraInicio.Value.ToString(@"hh\:mm");
            }

            if (_curso.HoraFin.HasValue)
            {
                mtbHoraFin.Text = _curso.HoraFin.Value.ToString(@"hh\:mm");
            }
        }

        private void ConfigureButtons()
        {
            bool isAdmin = _currentUser != null && _currentUser.RoleId == 1;
            btnInactivar.Visible = isAdmin && !_isNewCurso;
        }

        private void btnInactivar_Click(object sender, EventArgs e)
        {
            if (_curso == null || _curso.Id == 0)
            {
                return;
            }

            string message = $"¿Está seguro que desea inactivar el curso '{_curso.Nombre}'?";
            DialogResult result = MessageBox.Show(message, "Confirmar inactivación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool deleted = _cursoBLL.Delete(_curso);
                    if (deleted)
                    {
                        MessageBox.Show("Curso inactivado exitosamente.");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al inactivar el curso: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

            if (ValidateDocentesTraslape())
            {
                MessageBox.Show("Uno o más docentes tienen un curso asignado en el mismo día y horario.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
