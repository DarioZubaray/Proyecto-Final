using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
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
            CheckAulas();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
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

        private void LoadCursos()
        {
            dgvCursos.DataSource = null;
            var cursos = _cursoBLL.FindAll();

            var cursosGrid = cursos.Select(c => new
            {
                c.Id,
                c.Nombre,
                c.Descripcion,
                Aula = c.AulaNombre ?? "Sin asignar",
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

            dgvCursos.Columns["Id"].HeaderText = "ID";
            dgvCursos.Columns["Nombre"].HeaderText = "Nombre";
            dgvCursos.Columns["Descripcion"].HeaderText = "Descripción";
            dgvCursos.Columns["Aula"].HeaderText = "Aula";
            dgvCursos.Columns["FechaInicio"].HeaderText = "Fecha Inicio";
            dgvCursos.Columns["FechaFin"].HeaderText = "Fecha Fin";
            dgvCursos.Columns["DiaSemana"].HeaderText = "Día";
            dgvCursos.Columns["HoraInicio"].HeaderText = "Hora Inicio";
            dgvCursos.Columns["HoraFin"].HeaderText = "Hora Fin";
            dgvCursos.Columns["Docentes"].HeaderText = "Docentes";

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
                MessageBox.Show("Seleccione un curso para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var curso = _cursoBLL.FindById(cursoId);
            if (curso == null)
            {
                MessageBox.Show("No se pudo cargar el curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Seleccione un curso para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var curso = _cursoBLL.FindById(cursoId);
            if (curso == null)
            {
                MessageBox.Show("No se pudo cargar el curso.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string message = $"¿Está seguro que desea eliminar el curso '{curso.Nombre}'?";
            DialogResult result = MessageBox.Show(message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _cursoBLL.Delete(curso);
                if (deleted)
                {
                    MessageBox.Show("Curso eliminado exitosamente.");
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
                Aula = c.AulaNombre ?? "Sin asignar",
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
