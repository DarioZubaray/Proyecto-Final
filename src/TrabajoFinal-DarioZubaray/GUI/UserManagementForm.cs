using System;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class UserManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IUserBLL _userBLL;
        #endregion

        #region Constructores
        public UserManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _userBLL = ServiceLocator.CreateUserBLL();
            ApplyResources();
            LoadUsers();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.UserManagement_Title;
            btnNew.Text = Resources.UserManagement_New;
            btnEdit.Text = Resources.UserManagement_Edit;
            btnDelete.Text = Resources.UserManagement_Delete;
            btnSearch.Text = Resources.UserManagement_Search;
            lblSearch.Text = Resources.UserManagement_SearchLabel;
        }

        private void LoadUsers()
        {
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = _userBLL.FindAll();
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvUsers.Columns.Count == 0)
            {
                return;
            }

            dgvUsers.Columns["Id"].HeaderText = "ID";
            dgvUsers.Columns["UserName"].HeaderText = Resources.UserForm_UserNameLabel;
            dgvUsers.Columns["IsActive"].HeaderText = Resources.UserForm_IsActiveLabel;
            dgvUsers.Columns["RoleId"].HeaderText = Resources.UserForm_RoleLabel;
            dgvUsers.Columns["LastUpdate"].HeaderText = "Última Actualización";

            dgvUsers.Columns["PasswordHash"].Visible = false;
            dgvUsers.Columns["RetriesCount"].Visible = false;
            dgvUsers.Columns["CreatedAt"].Visible = false;
            dgvUsers.Columns["Language"].Visible = false;
            dgvUsers.Columns["Role"].Visible = false;

            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.ReadOnly = true;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
        }

        private UserBE GetSelectedUser()
        {
            if (dgvUsers.CurrentRow == null) return null;
            return dgvUsers.CurrentRow.DataBoundItem as UserBE;
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var form = new UserForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null)
            {
                MessageBox.Show(Resources.UserManagement_SelectUser);
                return;
            }

            using (var form = new UserForm(user))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null)
            {
                MessageBox.Show(Resources.UserManagement_SelectUser);
                return;
            }

            string message = string.Format(Resources.UserManagement_DeleteConfirm, user.UserName);
            DialogResult result = MessageBox.Show(message, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _userBLL.Delete(user);
                if (deleted)
                {
                    MessageBox.Show(Resources.UserManagement_DeleteSuccess);
                    LoadUsers();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadUsers();
                return;
            }

            dgvUsers.DataSource = null;
            dgvUsers.DataSource = _userBLL.FindByUserName(searchText);
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
