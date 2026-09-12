using System;
using System.Windows.Forms;

using BE.Entities;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ClassroomManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IClassroomBLL _aulaBLL;
        #endregion

        #region Constructores
        public ClassroomManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _aulaBLL = ServiceLocatorBLL.CreateAulaBLL();
            LoadAulas();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void LoadAulas()
        {
            dgvAulas.DataSource = null;
            dgvAulas.DataSource = _aulaBLL.FindAll();
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvAulas.Columns.Count == 0)
            {
                return;
            }

            dgvAulas.Columns["Id"].HeaderText = "ID";
            dgvAulas.Columns["Nombre"].HeaderText = "Nombre";
            dgvAulas.Columns["Capacidad"].HeaderText = "Capacidad";

            dgvAulas.Columns["IsActive"].Visible = false;
            dgvAulas.Columns["CreatedAt"].Visible = false;
            dgvAulas.Columns["LastUpdate"].Visible = false;

            dgvAulas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAulas.MultiSelect = false;
            dgvAulas.ReadOnly = true;
            dgvAulas.AllowUserToAddRows = false;
            dgvAulas.AllowUserToDeleteRows = false;
        }

        private ClassroomBE GetSelectedAula()
        {
            if (dgvAulas.CurrentRow == null)
            {
                return null;
            }

            return dgvAulas.CurrentRow.DataBoundItem as ClassroomBE;
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new ClassroomForm(theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAulas();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var aula = GetSelectedAula();
            if (aula == null)
            {
                MessageBox.Show("Seleccione un aula para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new ClassroomForm(aula, theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadAulas();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var aula = GetSelectedAula();
            if (aula == null)
            {
                MessageBox.Show("Seleccione un aula para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = $"¿Está seguro que desea eliminar el aula '{aula.Nombre}'?";
            DialogResult result = MessageBox.Show(message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _aulaBLL.Delete(aula);
                if (deleted)
                {
                    MessageBox.Show("Aula eliminada exitosamente.");
                    LoadAulas();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadAulas();
                return;
            }

            dgvAulas.DataSource = null;
            dgvAulas.DataSource = _aulaBLL.FindByName(searchText);
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
