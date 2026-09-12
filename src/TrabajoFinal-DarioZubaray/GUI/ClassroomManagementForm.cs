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
        private readonly IClassroomBLL _classroomBLL;
        #endregion

        #region Constructores
        public ClassroomManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            LoadClassrooms();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void LoadClassrooms()
        {
            dgvClassrooms.DataSource = null;
            dgvClassrooms.DataSource = _classroomBLL.FindAll();
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvClassrooms.Columns.Count == 0)
            {
                return;
            }

            dgvClassrooms.Columns["Id"].HeaderText = "ID";
            dgvClassrooms.Columns["Name"].HeaderText = "Nombre";
            dgvClassrooms.Columns["Capacity"].HeaderText = "Capacidad";

            dgvClassrooms.Columns["IsActive"].Visible = false;
            dgvClassrooms.Columns["CreatedAt"].Visible = false;
            dgvClassrooms.Columns["LastUpdate"].Visible = false;

            dgvClassrooms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClassrooms.MultiSelect = false;
            dgvClassrooms.ReadOnly = true;
            dgvClassrooms.AllowUserToAddRows = false;
            dgvClassrooms.AllowUserToDeleteRows = false;
        }

        private ClassroomBE GetSelectedClassroom()
        {
            if (dgvClassrooms.CurrentRow == null)
            {
                return null;
            }

            return dgvClassrooms.CurrentRow.DataBoundItem as ClassroomBE;
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
                    LoadClassrooms();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var classroom = GetSelectedClassroom();
            if (classroom == null)
            {
                MessageBox.Show("Seleccione un aula para editar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new ClassroomForm(classroom, theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClassrooms();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var classroom = GetSelectedClassroom();
            if (classroom == null)
            {
                MessageBox.Show("Seleccione un aula para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = $"¿Está seguro que desea eliminar el aula '{classroom.Name}'?";
            DialogResult result = MessageBox.Show(message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _classroomBLL.Delete(classroom);
                if (deleted)
                {
                    MessageBox.Show("Aula eliminada exitosamente.");
                    LoadClassrooms();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadClassrooms();
                return;
            }

            dgvClassrooms.DataSource = null;
            dgvClassrooms.DataSource = _classroomBLL.FindByName(searchText);
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
