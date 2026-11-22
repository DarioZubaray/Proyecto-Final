using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ReportForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _courseBLL;
        private readonly IReportBLL _reportBLL;
        private bool _reportGenerated;
        #endregion

        #region Constructor
        public ReportForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _reportBLL = ServiceLocatorBLL.CreateReportBLL();
            ApplyResources();
            LoadTypes();
            LoadCourses();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.ReportForm_Title;
            lblType.Text = Resources.ReportForm_TypeLabel;
            lblCourse.Text = Resources.ReportForm_CourseLabel;
            btnGenerate.Text = Resources.ReportForm_GenerateButton;
            btnExport.Text = Resources.ReportForm_ExportButton;
        }

        private void LoadTypes()
        {
            cboType.Items.Clear();
            cboType.Items.Add(Resources.ReportForm_TypeAttendance);
            cboType.Items.Add(Resources.ReportForm_TypeGrades);
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void LoadCourses()
        {
            List<CourseBE> courses = _courseBLL.FindAll();

            cboCourse.DataSource = null;
            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "Name";
            cboCourse.ValueMember = "Id";

            if (courses.Count == 0)
            {
                btnGenerate.Enabled = false;
                btnExport.Enabled = false;
                MessageBox.Show(Resources.ReportForm_NoCourses,
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ValidateInputs()
        {
            if (cboType.SelectedIndex < 0)
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.ReportForm_SelectType, ErrorCodesBLL.Validation.NoSelection),
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (cboCourse.SelectedValue == null)
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.ReportForm_SelectCourse, ErrorCodesBLL.Validation.NoSelection),
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void GenerateReport()
        {
            if (!ValidateInputs())
            {
                return;
            }

            int courseId = (int)cboCourse.SelectedValue;

            if (cboType.SelectedIndex == 0)
            {
                LoadAttendanceReport(courseId);
            }
            else
            {
                LoadGradesReport(courseId);
            }
        }

        private void LoadAttendanceReport(int courseId)
        {
            List<AttendanceReportLineBE> lines = _reportBLL.GetAttendanceReport(courseId);

            dgvReport.DataSource = null;
            dgvReport.Columns.Clear();
            _reportGenerated = false;

            if (lines.Count == 0)
            {
                MessageBox.Show(Resources.ReportForm_NoData,
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("Plans", typeof(int));
            dt.Columns.Add("Present", typeof(int));
            dt.Columns.Add("Attendance", typeof(string));

            foreach (AttendanceReportLineBE line in lines)
            {
                dt.Rows.Add(
                    line.StudentId,
                    line.StudentName,
                    line.TotalPlans,
                    line.PresentPlans,
                    line.AttendancePercent.HasValue ? line.AttendancePercent.Value.ToString() : "—");
            }

            dgvReport.DataSource = dt;

            dgvReport.Columns["StudentId"].Visible = false;
            dgvReport.Columns["Student"].HeaderText = Resources.ReportForm_ColStudent;
            dgvReport.Columns["Student"].ReadOnly = true;
            dgvReport.Columns["Student"].Width = 280;
            dgvReport.Columns["Plans"].HeaderText = Resources.ReportForm_ColPlans;
            dgvReport.Columns["Plans"].ReadOnly = true;
            dgvReport.Columns["Plans"].Width = 90;
            dgvReport.Columns["Present"].HeaderText = Resources.ReportForm_ColPresent;
            dgvReport.Columns["Present"].ReadOnly = true;
            dgvReport.Columns["Present"].Width = 90;
            dgvReport.Columns["Attendance"].HeaderText = Resources.ReportForm_ColAttendancePercent;
            dgvReport.Columns["Attendance"].ReadOnly = true;
            dgvReport.Columns["Attendance"].Width = 110;
            ConfigureGrid();
            _reportGenerated = true;
        }

        private void LoadGradesReport(int courseId)
        {
            List<GradeReportLineBE> lines = _reportBLL.GetGradesReport(courseId);

            dgvReport.DataSource = null;
            dgvReport.Columns.Clear();
            _reportGenerated = false;

            if (lines.Count == 0)
            {
                MessageBox.Show(Resources.ReportForm_NoData,
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("Evaluations", typeof(int));
            dt.Columns.Add("GradesCount", typeof(int));
            dt.Columns.Add("Average", typeof(string));

            foreach (GradeReportLineBE line in lines)
            {
                dt.Rows.Add(
                    line.StudentId,
                    line.StudentName,
                    line.EvaluationsCount,
                    line.GradesCount,
                    line.Average.HasValue ? line.Average.Value.ToString("0.##") : "—");
            }

            dgvReport.DataSource = dt;

            dgvReport.Columns["StudentId"].Visible = false;
            dgvReport.Columns["Student"].HeaderText = Resources.ReportForm_ColStudent;
            dgvReport.Columns["Student"].ReadOnly = true;
            dgvReport.Columns["Student"].Width = 280;
            dgvReport.Columns["Evaluations"].HeaderText = Resources.ReportForm_ColEvaluations;
            dgvReport.Columns["Evaluations"].ReadOnly = true;
            dgvReport.Columns["Evaluations"].Width = 100;
            dgvReport.Columns["GradesCount"].HeaderText = Resources.ReportForm_ColGradesCount;
            dgvReport.Columns["GradesCount"].ReadOnly = true;
            dgvReport.Columns["GradesCount"].Width = 110;
            dgvReport.Columns["Average"].HeaderText = Resources.ReportForm_ColAverage;
            dgvReport.Columns["Average"].ReadOnly = true;
            dgvReport.Columns["Average"].Width = 130;
            ConfigureGrid();
            _reportGenerated = true;
        }

        private void ConfigureGrid()
        {
            dgvReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReport.MultiSelect = false;
            dgvReport.AllowUserToAddRows = false;
            dgvReport.AllowUserToDeleteRows = false;
            dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void ExportReport()
        {
            if (!_reportGenerated || dgvReport.Rows.Count == 0)
            {
                MessageBox.Show(Resources.ReportForm_ExportBeforeGenerate,
                    Resources.ReportForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV (*.csv)|*.csv";
                dialog.DefaultExt = "csv";

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                StringBuilder content = new StringBuilder();
                List<string> headers = new List<string>();

                foreach (DataGridViewColumn column in dgvReport.Columns)
                {
                    if (column.Visible)
                    {
                        headers.Add(column.HeaderText);
                    }
                }

                content.AppendLine(string.Join(";", headers));

                foreach (DataGridViewRow row in dgvReport.Rows)
                {
                    List<string> values = new List<string>();

                    foreach (DataGridViewColumn column in dgvReport.Columns)
                    {
                        if (column.Visible)
                        {
                            string cellValue = Convert.ToString(row.Cells[column.Name].Value) ?? string.Empty;
                            values.Add(cellValue.Replace('.', ','));
                        }
                    }

                    content.AppendLine(string.Join(";", values));
                }

                File.WriteAllText(dialog.FileName, content.ToString(), Encoding.UTF8);

                MessageBox.Show(Resources.ReportForm_Exported, this.Text,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Eventos
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportReport();
        }
        #endregion
    }
}
