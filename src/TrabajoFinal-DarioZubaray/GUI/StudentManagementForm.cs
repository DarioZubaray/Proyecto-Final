using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class StudentManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IStudentBLL _studentBLL;
        #endregion

        #region Constructores
        public StudentManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _studentBLL = ServiceLocatorBLL.CreateStudentBLL();
            ApplyResources();
            LoadStudents();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.StudentManagementForm_Title;
            lblSearch.Text = Resources.StudentManagementForm_SearchLabel;
            btnSearch.Text = Resources.StudentManagementForm_SearchButton;
            btnNew.Text = Resources.StudentManagementForm_NewButton;
            btnEdit.Text = Resources.StudentManagementForm_EditButton;
            btnDelete.Text = Resources.StudentManagementForm_DeleteButton;
        }

        private void LoadStudents()
        {
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = _studentBLL.FindAll();
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvStudents.Columns.Count == 0)
            {
                return;
            }

            dgvStudents.Columns["Id"].HeaderText = "ID";
            dgvStudents.Columns["FirstName"].HeaderText = Resources.StudentManagementForm_ColFirstName;
            dgvStudents.Columns["LastName"].HeaderText = Resources.StudentManagementForm_ColLastName;
            dgvStudents.Columns["Document"].HeaderText = Resources.StudentManagementForm_ColDocument;
            dgvStudents.Columns["Email"].HeaderText = Resources.StudentManagementForm_ColEmail;
            dgvStudents.Columns["Phone"].HeaderText = Resources.StudentManagementForm_ColPhone;
            dgvStudents.Columns["IsActive"].HeaderText = Resources.StudentManagementForm_ColActive;

            dgvStudents.Columns["CreatedAt"].Visible = false;
            dgvStudents.Columns["LastUpdate"].Visible = false;
            dgvStudents.Columns["FullName"].Visible = false;

            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStudents.MultiSelect = false;
            dgvStudents.ReadOnly = true;
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.AllowUserToDeleteRows = false;
        }

        private StudentBE GetSelectedStudent()
        {
            if (dgvStudents.CurrentRow == null)
            {
                return null;
            }

            return dgvStudents.CurrentRow.DataBoundItem as StudentBE;
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new StudentForm(theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStudents();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var student = GetSelectedStudent();
            if (student == null)
            {
                MessageBox.Show(Resources.StudentManagementForm_SelectToEdit, Resources.StudentManagementForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new StudentForm(student, theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadStudents();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var student = GetSelectedStudent();
            if (student == null)
            {
                MessageBox.Show(Resources.StudentManagementForm_SelectToDelete, Resources.StudentManagementForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = string.Format(Resources.StudentManagementForm_ConfirmDeleteMessage, student.FullName);
            DialogResult result = MessageBox.Show(message, Resources.StudentManagementForm_ConfirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _studentBLL.Delete(student);
                if (deleted)
                {
                    MessageBox.Show(Resources.StudentManagementForm_DeleteSuccess);
                    LoadStudents();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadStudents();
                return;
            }

            dgvStudents.DataSource = null;
            dgvStudents.DataSource = _studentBLL.FindByName(searchText);
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
