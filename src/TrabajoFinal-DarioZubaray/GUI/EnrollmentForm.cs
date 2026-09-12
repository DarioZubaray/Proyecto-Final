using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class EnrollmentForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _courseBLL;
        private readonly IEnrollmentBLL _enrollmentBLL;
        #endregion

        #region Constructor
        public EnrollmentForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _enrollmentBLL = ServiceLocatorBLL.CreateEnrollmentBLL();
            ApplyResources();
            LoadData();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.EnrollmentForm_Title;
            lblAvailableCourses.Text = Resources.EnrollmentForm_AvailableCoursesLabel;
            lblMyEnrollments.Text = Resources.EnrollmentForm_MyEnrollmentsLabel;
            btnEnroll.Text = Resources.EnrollmentForm_EnrollButton;
            btnUnenroll.Text = Resources.EnrollmentForm_UnenrollButton;
        }

        private string GetDayOfWeekName(int? dia)
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

        private void LoadData()
        {
            LoadAvailableCourses();
            LoadMyEnrollments();
        }

        private void LoadAvailableCourses()
        {
            var allCourses = _courseBLL.FindAll();
            var myEnrollments = _enrollmentBLL.FindByStudentId(_currentUser.Id);
            var enrolledCourseIds = myEnrollments.Select(i => i.CourseId).ToHashSet();

            var availableCourses = allCourses
                .Where(c => !enrolledCourseIds.Contains(c.Id))
                .ToList();

            var coursesGrid = availableCourses.Select(c => new
            {
                c.Id,
                Name = c.Name,
                Classroom = c.ClassroomName ?? Resources.EnrollmentForm_NoAssignment,
                DayOfWeek = GetDayOfWeekName(c.DayOfWeek),
                StartTime = c.StartTime.HasValue ? c.StartTime.Value.ToString(@"hh\:mm") : "-",
                EndTime = c.EndTime.HasValue ? c.EndTime.Value.ToString(@"hh\:mm") : "-",
                Teachers = string.Join(", ", c.Teachers.Select(d => d.UserName))
            }).ToList();

            dgvAvailableCourses.DataSource = coursesGrid;
            ConfigureCoursesGrid();
        }

        private void LoadMyEnrollments()
        {
            var enrollments = _enrollmentBLL.FindByStudentId(_currentUser.Id);

            var enrollmentsGrid = enrollments.Select(i => new
            {
                i.CourseId,
                Course = i.CourseName,
                DayOfWeek = GetDayOfWeekName(i.DayOfWeek),
                StartTime = i.StartTime.HasValue ? i.StartTime.Value.ToString(@"hh\:mm") : "-",
                EndTime = i.EndTime.HasValue ? i.EndTime.Value.ToString(@"hh\:mm") : "-",
                Classroom = i.ClassroomName ?? Resources.EnrollmentForm_NoAssignment
            }).ToList();

            dgvMyEnrollments.DataSource = enrollmentsGrid;
            ConfigureEnrollmentsGrid();
        }

        private void ConfigureCoursesGrid()
        {
            if (dgvAvailableCourses.Columns.Count == 0) return;

            dgvAvailableCourses.Columns["Id"].HeaderText = Resources.EnrollmentForm_ColId;
            dgvAvailableCourses.Columns["Name"].HeaderText = Resources.EnrollmentForm_ColCourse;
            dgvAvailableCourses.Columns["Classroom"].HeaderText = Resources.EnrollmentForm_ColClassroom;
            dgvAvailableCourses.Columns["DayOfWeek"].HeaderText = Resources.EnrollmentForm_ColDay;
            dgvAvailableCourses.Columns["StartTime"].HeaderText = Resources.EnrollmentForm_ColStartTime;
            dgvAvailableCourses.Columns["EndTime"].HeaderText = Resources.EnrollmentForm_ColEndTime;
            dgvAvailableCourses.Columns["Teachers"].HeaderText = Resources.EnrollmentForm_ColTeachers;

            dgvAvailableCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAvailableCourses.MultiSelect = false;
            dgvAvailableCourses.ReadOnly = true;
            dgvAvailableCourses.AllowUserToAddRows = false;
            dgvAvailableCourses.AllowUserToDeleteRows = false;
        }

        private void ConfigureEnrollmentsGrid()
        {
            if (dgvMyEnrollments.Columns.Count == 0) return;

            dgvMyEnrollments.Columns["CourseId"].HeaderText = Resources.EnrollmentForm_ColId;
            dgvMyEnrollments.Columns["Course"].HeaderText = Resources.EnrollmentForm_ColCourse;
            dgvMyEnrollments.Columns["DayOfWeek"].HeaderText = Resources.EnrollmentForm_ColDay;
            dgvMyEnrollments.Columns["StartTime"].HeaderText = Resources.EnrollmentForm_ColStartTime;
            dgvMyEnrollments.Columns["EndTime"].HeaderText = Resources.EnrollmentForm_ColEndTime;
            dgvMyEnrollments.Columns["Classroom"].HeaderText = Resources.EnrollmentForm_ColClassroom;

            dgvMyEnrollments.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMyEnrollments.MultiSelect = false;
            dgvMyEnrollments.ReadOnly = true;
            dgvMyEnrollments.AllowUserToAddRows = false;
            dgvMyEnrollments.AllowUserToDeleteRows = false;
        }
        #endregion

        #region Eventos
        private void btnEnroll_Click(object sender, EventArgs e)
        {
            if (dgvAvailableCourses.CurrentRow == null)
            {
                MessageBox.Show(Resources.EnrollmentForm_SelectCourseToEnroll, Resources.EnrollmentForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int courseId = Convert.ToInt32(dgvAvailableCourses.CurrentRow.Cells["Id"].Value);
            string courseName = dgvAvailableCourses.CurrentRow.Cells["Name"].Value.ToString();

            string confirmMessage = string.Format(Resources.EnrollmentForm_ConfirmEnrollMessage, courseName);
            DialogResult result = MessageBox.Show(
                confirmMessage,
                Resources.EnrollmentForm_ConfirmEnrollTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _enrollmentBLL.Enroll(courseId, _currentUser.Id);
                    MessageBox.Show(Resources.EnrollmentForm_EnrollSuccess, Resources.EnrollmentForm_SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(ex.Message, Resources.EnrollmentForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format(Resources.EnrollmentForm_EnrollError, ex.Message);
                    MessageBox.Show(errorMsg, Resources.EnrollmentForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUnenroll_Click(object sender, EventArgs e)
        {
            if (dgvMyEnrollments.CurrentRow == null)
            {
                MessageBox.Show(Resources.EnrollmentForm_SelectEnrollmentToUnenroll, Resources.EnrollmentForm_InfoTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int courseId = Convert.ToInt32(dgvMyEnrollments.CurrentRow.Cells["CourseId"].Value);
            string courseName = dgvMyEnrollments.CurrentRow.Cells["Course"].Value.ToString();

            string confirmMessage = string.Format(Resources.EnrollmentForm_ConfirmUnenrollMessage, courseName);
            DialogResult result = MessageBox.Show(
                confirmMessage,
                Resources.EnrollmentForm_ConfirmUnenrollTitle,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _enrollmentBLL.Unenroll(courseId, _currentUser.Id);
                    MessageBox.Show(Resources.EnrollmentForm_UnenrollSuccess, Resources.EnrollmentForm_SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    string errorMsg = string.Format(Resources.EnrollmentForm_UnenrollError, ex.Message);
                    MessageBox.Show(errorMsg, Resources.EnrollmentForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
