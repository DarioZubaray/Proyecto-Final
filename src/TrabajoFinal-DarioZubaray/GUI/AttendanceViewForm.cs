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
    public partial class AttendanceViewForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IAttendanceBLL _attendanceBLL;
        private readonly ICourseBLL _courseBLL;
        private readonly IEnrollmentBLL _enrollmentBLL;
        #endregion

        #region Constructor
        public AttendanceViewForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _attendanceBLL = ServiceLocatorBLL.CreateAttendanceBLL();
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _enrollmentBLL = ServiceLocatorBLL.CreateEnrollmentBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
            LoadCourses();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.AttendanceViewForm_Title;
            lblCourse.Text = Resources.AttendanceForm_CourseLabel;
            lblDate.Text = Resources.AttendanceForm_DateLabel;
            btnSearch.Text = Resources.AttendanceForm_SearchButton;
        }

        private void LoadCourses()
        {
            var courses = _courseBLL.FindAll();

            cboCourse.DataSource = null;
            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "Name";
            cboCourse.ValueMember = "Id";

            if (courses.Count == 0)
            {
                btnSearch.Enabled = false;
            }
        }

        private void LoadStudents()
        {
            if (cboCourse.SelectedValue == null) return;

            int courseId = (int)cboCourse.SelectedValue;
            DateTime date = dtpDate.Value.Date;

            dgvAttendance.DataSource = null;
            dgvAttendance.Columns.Clear();

            List<EnrollmentBE> enrollments = _enrollmentBLL.FindAll()
                .Where(i => i.CourseId == courseId && i.IsActive)
                .ToList();

            if (enrollments.Count == 0)
            {
                MessageBox.Show(Resources.AttendanceForm_NoStudents,
                    Resources.AttendanceForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<AttendanceBE> existingAttendance = _attendanceBLL.FindByCourseIdAndDate(courseId, date);
            Dictionary<int, bool> attendanceMap = existingAttendance.ToDictionary(a => a.StudentId, a => a.IsPresent);

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("IsPresent", typeof(bool));

            foreach (var enrollment in enrollments)
            {
                bool isPresent = attendanceMap.ContainsKey(enrollment.StudentId) && attendanceMap[enrollment.StudentId];
                dt.Rows.Add(enrollment.StudentId, enrollment.StudentName, isPresent);
            }

            dgvAttendance.DataSource = dt;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvAttendance.Columns.Count == 0) return;

            dgvAttendance.Columns["StudentId"].Visible = false;
            dgvAttendance.Columns["Student"].HeaderText = Resources.AttendanceForm_ColStudent;
            dgvAttendance.Columns["Student"].ReadOnly = true;
            dgvAttendance.Columns["Student"].Width = 400;

            dgvAttendance.Columns["IsPresent"].HeaderText = Resources.AttendanceForm_ColPresent;
            dgvAttendance.Columns["IsPresent"].ReadOnly = true;
            dgvAttendance.Columns["IsPresent"].Width = 100;
            dgvAttendance.Columns["IsPresent"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAttendance.Columns["IsPresent"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAttendance.MultiSelect = false;
            dgvAttendance.AllowUserToAddRows = false;
            dgvAttendance.AllowUserToDeleteRows = false;
            dgvAttendance.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }
        #endregion

        #region Eventos
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cboCourse.SelectedValue == null)
            {
                MessageBox.Show(Resources.AttendanceForm_SelectCourse,
                    Resources.AttendanceForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadStudents();
        }
        #endregion
    }
}
