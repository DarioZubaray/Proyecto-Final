using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
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
            ApplyResources();
            LoadClassrooms();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.ClassroomManagementForm_Title;
            lblSearch.Text = Resources.ClassroomManagementForm_SearchLabel;
            btnSearch.Text = Resources.ClassroomManagementForm_SearchButton;
            btnNew.Text = Resources.ClassroomManagementForm_NewButton;
            btnEdit.Text = Resources.ClassroomManagementForm_EditButton;
            btnDelete.Text = Resources.ClassroomManagementForm_DeleteButton;
        }

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
            dgvClassrooms.Columns["Name"].HeaderText = Resources.ClassroomManagementForm_ColName;
            dgvClassrooms.Columns["Capacity"].HeaderText = Resources.ClassroomManagementForm_ColCapacity;

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
                MessageBox.Show(Resources.ClassroomManagementForm_SelectToEdit, Resources.ClassroomManagementForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(Resources.ClassroomManagementForm_SelectToDelete, Resources.ClassroomManagementForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = string.Format(Resources.ClassroomManagementForm_ConfirmDeleteMessage, classroom.Name);
            DialogResult result = MessageBox.Show(message, Resources.ClassroomManagementForm_ConfirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _classroomBLL.Delete(classroom);
                if (deleted)
                {
                    MessageBox.Show(Resources.ClassroomManagementForm_DeleteSuccess);
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
