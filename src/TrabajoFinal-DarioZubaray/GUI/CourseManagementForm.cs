using System;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class CourseManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _courseBLL;
        private readonly IClassroomBLL _classroomBLL;
        #endregion

        #region Constructores
        public CourseManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            ApplyResources();
            CheckClassrooms();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.CourseManagementForm_Title;
            lblSearch.Text = Resources.CourseManagementForm_SearchLabel;
            btnSearch.Text = Resources.CourseManagementForm_SearchButton;
            btnNew.Text = Resources.CourseManagementForm_NewButton;
            btnEdit.Text = Resources.CourseManagementForm_EditButton;
            btnDelete.Text = Resources.CourseManagementForm_DeleteButton;
            lblNoClassrooms.Text = Resources.CourseManagementForm_NoAulasMessage;
        }

        private void CheckClassrooms()
        {
            int classroomCount = _classroomBLL.Count();

            if (classroomCount == 0)
            {
                dgvCourses.Visible = false;
                panelButtons.Visible = false;
                panelTop.Visible = false;
                lblNoClassrooms.Visible = true;
            }
            else
            {
                dgvCourses.Visible = true;
                panelButtons.Visible = true;
                panelTop.Visible = true;
                lblNoClassrooms.Visible = false;
                LoadCourses();
            }
        }

        private string GetDayOfWeekName(int? day)
        {
            if (!day.HasValue) return "-";
            switch (day.Value)
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

        private void LoadCourses()
        {
            dgvCourses.DataSource = null;
            var courses = _courseBLL.FindAllIncludingInactive();

            var coursesGrid = courses.Select(c => new
            {
                c.Id,
                Name = c.Name,
                Description = c.Description,
                Classroom = c.ClassroomName ?? Resources.CourseManagementForm_NoClassroom,
                Status = c.IsActive ? Resources.CourseManagementForm_StatusActive : Resources.CourseManagementForm_StatusInactive,
                StartDate = c.StartDate.ToString("dd/MM/yyyy"),
                EndDate = c.EndDate.ToString("dd/MM/yyyy"),
                DayOfWeek = GetDayOfWeekName(c.DayOfWeek),
                StartTime = c.StartTime.HasValue ? c.StartTime.Value.ToString(@"hh\:mm") : "-",
                EndTime = c.EndTime.HasValue ? c.EndTime.Value.ToString(@"hh\:mm") : "-",
                Teachers = string.Join(", ", c.Teachers.Select(d => d.UserName))
            }).ToList();

            dgvCourses.DataSource = coursesGrid;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvCourses.Columns.Count == 0)
            {
                return;
            }

            dgvCourses.Columns["Id"].HeaderText = Resources.CourseManagementForm_ColId;
            dgvCourses.Columns["Name"].HeaderText = Resources.CourseManagementForm_ColName;
            dgvCourses.Columns["Description"].HeaderText = Resources.CourseManagementForm_ColDescription;
            dgvCourses.Columns["Classroom"].HeaderText = Resources.CourseManagementForm_ColClassroom;
            dgvCourses.Columns["Status"].HeaderText = Resources.CourseManagementForm_ColStatus;
            dgvCourses.Columns["StartDate"].HeaderText = Resources.CourseManagementForm_ColStartDate;
            dgvCourses.Columns["EndDate"].HeaderText = Resources.CourseManagementForm_ColEndDate;
            dgvCourses.Columns["DayOfWeek"].HeaderText = Resources.CourseManagementForm_ColDay;
            dgvCourses.Columns["StartTime"].HeaderText = Resources.CourseManagementForm_ColStartTime;
            dgvCourses.Columns["EndTime"].HeaderText = Resources.CourseManagementForm_ColEndTime;
            dgvCourses.Columns["Teachers"].HeaderText = Resources.CourseManagementForm_ColTeachers;

            foreach (DataGridViewColumn column in dgvCourses.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.MultiSelect = false;
            dgvCourses.ReadOnly = true;
            dgvCourses.AllowUserToAddRows = false;
            dgvCourses.AllowUserToDeleteRows = false;
        }

        private int GetSelectedCourseId()
        {
            if (dgvCourses.CurrentRow == null)
            {
                return 0;
            }

            return Convert.ToInt32(dgvCourses.CurrentRow.Cells["Id"].Value);
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new CourseForm(theme, _currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCourses();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int courseId = GetSelectedCourseId();
            if (courseId == 0)
            {
                MessageBox.Show(Resources.CourseManagementForm_SelectToEdit, Resources.EnrollmentForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var course = _courseBLL.FindById(courseId);
            if (course == null)
            {
                MessageBox.Show(Resources.CourseManagementForm_CourseLoadError, Resources.EnrollmentForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new CourseForm(course, theme, _currentUser))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCourses();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int courseId = GetSelectedCourseId();
            if (courseId == 0)
            {
                MessageBox.Show(Resources.CourseManagementForm_SelectToDelete, Resources.EnrollmentForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var course = _courseBLL.FindById(courseId);
            if (course == null)
            {
                MessageBox.Show(Resources.CourseManagementForm_CourseLoadError, Resources.EnrollmentForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string message = string.Format(Resources.CourseManagementForm_ConfirmDeleteMessage, course.Name);
            DialogResult result = MessageBox.Show(message, Resources.CourseManagementForm_ConfirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bool deleted = _courseBLL.Delete(course);
                if (deleted)
                {
                    MessageBox.Show(Resources.CourseManagementForm_DeleteSuccess);
                    LoadCourses();
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadCourses();
                return;
            }

            dgvCourses.DataSource = null;
            var courses = _courseBLL.FindByName(searchText);

            var coursesGrid = courses.Select(c => new
            {
                c.Id,
                Name = c.Name,
                Description = c.Description,
                Classroom = c.ClassroomName ?? Resources.CourseManagementForm_NoClassroom,
                Status = c.IsActive ? Resources.CourseManagementForm_StatusActive : Resources.CourseManagementForm_StatusInactive,
                StartDate = c.StartDate.ToString("dd/MM/yyyy"),
                EndDate = c.EndDate.ToString("dd/MM/yyyy"),
                DayOfWeek = GetDayOfWeekName(c.DayOfWeek),
                StartTime = c.StartTime.HasValue ? c.StartTime.Value.ToString(@"hh\:mm") : "-",
                EndTime = c.EndTime.HasValue ? c.EndTime.Value.ToString(@"hh\:mm") : "-",
                Teachers = string.Join(", ", c.Teachers.Select(d => d.UserName))
            }).ToList();

            dgvCourses.DataSource = coursesGrid;
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
