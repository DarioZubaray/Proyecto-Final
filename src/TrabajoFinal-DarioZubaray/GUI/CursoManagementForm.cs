using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class CursoManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICursoBLL _cursoBLL;
        private readonly IAulaBLL _aulaBLL;
        #endregion

        #region Constructores
        public CursoManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _cursoBLL = ServiceLocatorBLL.CreateCursoBLL();
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            ApplyResources();
            CheckAulas();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.CursoManagementForm_Title;
            lblSearch.Text = Resources.CursoManagementForm_SearchLabel;
            btnSearch.Text = Resources.CursoManagementForm_SearchButton;
            btnNew.Text = Resources.CursoManagementForm_NewButton;
            btnEdit.Text = Resources.CursoManagementForm_EditButton;
            btnDelete.Text = Resources.CursoManagementForm_DeleteButton;
            lblNoAulas.Text = Resources.CursoManagementForm_NoAulasMessage;
        }

        private void CheckAulas()
        {
            int aulasCount = _aulaBLL.Count();

            if (aulasCount == 0)
            {
                dgvCursos.Visible = false;
                panelButtons.Visible = false;
                panelTop.Visible = false;
                lblNoAulas.Visible = true;
            }
            else
            {
                dgvCursos.Visible = true;
                panelButtons.Visible = true;
                panelTop.Visible = true;
                lblNoAulas.Visible = false;
                LoadCursos();
            }
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

        private void LoadCursos()
        {
            dgvCursos.DataSource = null;
            var cursos = _cursoBLL.FindAllIncludingInactive();

            var cursosGrid = cursos.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Descripcion,
                Aula = c.AulaNombre ?? Resources.CursoManagementForm_NoClassroom,
                Estado = c.IsActive ? Resources.CursoManagementForm_StatusActive : Resources.CursoManagementForm_StatusInactive,
                FechaInicio = c.FechaInicio.ToString("dd/MM/yyyy"),
                FechaFin = c.FechaFin.ToString("dd/MM/yyyy"),
                DiaSemana = GetDiaSemanaNombre(c.DiaSemana),
                HoraInicio = c.HoraInicio.HasValue ? c.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = c.HoraFin.HasValue ? c.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Docentes = string.Join(", ", c.Docentes.Select(d => d.UserName))
            }).ToList();

            dgvCursos.DataSource = cursosGrid;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvCursos.Columns.Count == 0)
            {
                return;
            }

            dgvCursos.Columns["Id"].HeaderText = Resources.CursoManagementForm_ColId;
            dgvCursos.Columns["Nombre"].HeaderText = Resources.CursoManagementForm_ColName;
            dgvCursos.Columns["Descripcion"].HeaderText = Resources.CursoManagementForm_ColDescription;
            dgvCursos.Columns["Aula"].HeaderText = Resources.CursoManagementForm_ColClassroom;
            dgvCursos.Columns["Estado"].HeaderText = Resources.CursoManagementForm_ColStatus;
            dgvCursos.Columns["FechaInicio"].HeaderText = Resources.CursoManagementForm_ColStartDate;
            dgvCursos.Columns["FechaFin"].HeaderText = Resources.CursoManagementForm_ColEndDate;
            dgvCursos.Columns["DiaSemana"].HeaderText = Resources.CursoManagementForm_ColDay;
            dgvCursos.Columns["HoraInicio"].HeaderText = Resources.CursoManagementForm_ColStartTime;
            dgvCursos.Columns["HoraFin"].HeaderText = Resources.CursoManagementForm_ColEndTime;
            dgvCursos.Columns["Docentes"].HeaderText = Resources.CursoManagementForm_ColTeachers;

            foreach (DataGridViewColumn column in dgvCursos.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            dgvCursos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCursos.MultiSelect = false;
            dgvCursos.ReadOnly = true;
            dgvCursos.AllowUserToAddRows = false;
            dgvCursos.AllowUserToDeleteRows = false;
        }

        private int GetSelectedCursoId()
        {
            if (dgvCursos.CurrentRow == null)
            {
                return 0;
            }

            return Convert.ToInt32(dgvCursos.CurrentRow.Cells["Id"].Value);
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new CursoForm(theme, _currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCursos();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int cursoId = GetSelectedCursoId();
            if (cursoId == 0)
            {
                MessageBox.Show(Resources.CursoManagementForm_SelectToEdit, Resources.InscripcionForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var curso = _cursoBLL.FindById(cursoId);
            if (curso == null)
            {
                MessageBox.Show(Resources.CursoManagementForm_CourseLoadError, Resources.InscripcionForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new CursoForm(curso, theme, _currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCursos();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int cursoId = GetSelectedCursoId();
            if (cursoId == 0)
            {
                MessageBox.Show(Resources.CursoManagementForm_SelectToDelete, Resources.InscripcionForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var curso = _cursoBLL.FindById(cursoId);
            if (curso == null)
            {
                MessageBox.Show(Resources.CursoManagementForm_CourseLoadError, Resources.InscripcionForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string message = string.Format(Resources.CursoManagementForm_ConfirmDeleteMessage, curso.Nombre);
            DialogResult result = MessageBox.Show(message, Resources.CursoManagementForm_ConfirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _cursoBLL.Delete(curso);
                if (deleted)
                {
                    MessageBox.Show(Resources.CursoManagementForm_DeleteSuccess);
                    LoadCursos();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadCursos();
                return;
            }

            dgvCursos.DataSource = null;
            var cursos = _cursoBLL.FindByName(searchText);

            var cursosGrid = cursos.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Descripcion,
                Aula = c.AulaNombre ?? Resources.CursoManagementForm_NoClassroom,
                Estado = c.IsActive ? Resources.CursoManagementForm_StatusActive : Resources.CursoManagementForm_StatusInactive,
                FechaInicio = c.FechaInicio.ToString("dd/MM/yyyy"),
                FechaFin = c.FechaFin.ToString("dd/MM/yyyy"),
                DiaSemana = GetDiaSemanaNombre(c.DiaSemana),
                HoraInicio = c.HoraInicio.HasValue ? c.HoraInicio.Value.ToString(@"hh\:mm") : "-",
                HoraFin = c.HoraFin.HasValue ? c.HoraFin.Value.ToString(@"hh\:mm") : "-",
                Docentes = string.Join(", ", c.Docentes.Select(d => d.UserName))
            }).ToList();

            dgvCursos.DataSource = cursosGrid;
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
