using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class PerformanceForm : Form
    {
        #region Propiedades
        private const int ROLE_PROFESOR = 2;
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _courseBLL;
        private readonly IReportBLL _reportBLL;
        #endregion

        #region Constructor
        public PerformanceForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _reportBLL = ServiceLocatorBLL.CreateReportBLL();
            ApplyResources();
            LoadCourses();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.PerformanceForm_Title;
            lblCourse.Text = Resources.PerformanceForm_CourseLabel;
            btnConsult.Text = Resources.PerformanceForm_ConsultButton;
        }

        private void LoadCourses()
        {
            List<CourseBE> courses = _currentUser.RoleId == ROLE_PROFESOR
                ? _courseBLL.FindByTeacherId(_currentUser.Id)
                : _courseBLL.FindAll();

            cboCourse.DataSource = null;
            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "Name";
            cboCourse.ValueMember = "Id";

            if (courses.Count == 0)
            {
                btnConsult.Enabled = false;
                MessageBox.Show(Resources.PerformanceForm_NoCourses,
                    Resources.PerformanceForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadPerformance(int courseId)
        {
            List<StudentPerformanceBE> performance = _reportBLL.GetCoursePerformance(courseId);

            dgvPerformance.DataSource = null;
            dgvPerformance.Columns.Clear();

            if (performance.Count == 0)
            {
                MessageBox.Show(Resources.PerformanceForm_NoStudents,
                    Resources.PerformanceForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("Attendance", typeof(string));
            dt.Columns.Add("GradesCount", typeof(string));
            dt.Columns.Add("Average", typeof(string));

            foreach (StudentPerformanceBE line in performance)
            {
                dt.Rows.Add(
                    line.StudentId,
                    line.StudentName,
                    line.AttendancePercent.HasValue ? line.AttendancePercent.Value.ToString() : "—",
                    line.GradesCount > 0 ? line.GradesCount.ToString() : "—",
                    line.Average.HasValue ? line.Average.Value.ToString("0.##") : "—");
            }

            dgvPerformance.DataSource = dt;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvPerformance.Columns.Count == 0)
            {
                return;
            }

            dgvPerformance.Columns["StudentId"].Visible = false;
            dgvPerformance.Columns["Student"].HeaderText = Resources.PerformanceForm_ColStudent;
            dgvPerformance.Columns["Student"].ReadOnly = true;
            dgvPerformance.Columns["Student"].Width = 300;
            dgvPerformance.Columns["Attendance"].HeaderText = Resources.PerformanceForm_ColAttendance;
            dgvPerformance.Columns["Attendance"].ReadOnly = true;
            dgvPerformance.Columns["Attendance"].Width = 110;
            dgvPerformance.Columns["Attendance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPerformance.Columns["GradesCount"].HeaderText = Resources.PerformanceForm_ColGradesCount;
            dgvPerformance.Columns["GradesCount"].ReadOnly = true;
            dgvPerformance.Columns["GradesCount"].Width = 110;
            dgvPerformance.Columns["GradesCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPerformance.Columns["Average"].HeaderText = Resources.PerformanceForm_ColAverage;
            dgvPerformance.Columns["Average"].ReadOnly = true;
            dgvPerformance.Columns["Average"].Width = 130;
            dgvPerformance.Columns["Average"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvPerformance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPerformance.MultiSelect = false;
            dgvPerformance.AllowUserToAddRows = false;
            dgvPerformance.AllowUserToDeleteRows = false;
        }
        #endregion

        #region Eventos
        private void btnConsult_Click(object sender, EventArgs e)
        {
            if (cboCourse.SelectedValue == null)
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.PerformanceForm_SelectCourse, ErrorCodesBLL.Validation.NoSelection),
                    Resources.PerformanceForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            LoadPerformance((int)cboCourse.SelectedValue);
        }
        #endregion
    }
}
