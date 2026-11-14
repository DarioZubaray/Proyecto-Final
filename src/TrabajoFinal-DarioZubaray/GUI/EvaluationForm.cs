using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class EvaluationForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ICourseBLL _courseBLL;
        private readonly IEnrollmentBLL _enrollmentBLL;
        private readonly IEvaluationBLL _evaluationBLL;
        private List<CourseBE> _teacherCourses;
        #endregion

        #region Constructor
        public EvaluationForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _enrollmentBLL = ServiceLocatorBLL.CreateEnrollmentBLL();
            _evaluationBLL = ServiceLocatorBLL.CreateEvaluationBLL();
            ApplyResources();
            LoadCourses();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.EvaluationForm_Title;
            lblCourse.Text = Resources.EvaluationForm_CourseLabel;
            lblName.Text = Resources.EvaluationForm_NameLabel;
            lblDate.Text = Resources.EvaluationForm_DateLabel;
            btnLoadStudents.Text = Resources.EvaluationForm_LoadStudentsButton;
            btnSave.Text = Resources.EvaluationForm_SaveButton;
        }

        private void LoadCourses()
        {
            _teacherCourses = _courseBLL.FindByTeacherId(_currentUser.Id);

            cboCourse.DataSource = null;
            cboCourse.DataSource = _teacherCourses;
            cboCourse.DisplayMember = "Name";
            cboCourse.ValueMember = "Id";

            if (_teacherCourses.Count == 0)
            {
                btnLoadStudents.Enabled = false;
                btnSave.Enabled = false;
                MessageBox.Show(Resources.EvaluationForm_NoCourses,
                    Resources.EvaluationForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadStudents()
        {
            if (cboCourse.SelectedValue == null)
            {
                return;
            }

            int courseId = (int)cboCourse.SelectedValue;

            dgvGrades.DataSource = null;
            dgvGrades.Columns.Clear();

            List<EnrollmentBE> enrollments = _enrollmentBLL.FindAll()
                .Where(i => i.CourseId == courseId && i.IsActive)
                .ToList();

            if (enrollments.Count == 0)
            {
                MessageBox.Show(Resources.EvaluationForm_NoStudents,
                    Resources.EvaluationForm_ValidationTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("StudentId", typeof(int));
            dt.Columns.Add("Student", typeof(string));
            dt.Columns.Add("Grade", typeof(string));

            foreach (EnrollmentBE enrollment in enrollments)
            {
                dt.Rows.Add(enrollment.StudentId, enrollment.StudentName, string.Empty);
            }

            dgvGrades.DataSource = dt;
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvGrades.Columns.Count == 0)
            {
                return;
            }

            dgvGrades.Columns["StudentId"].Visible = false;
            dgvGrades.Columns["Student"].HeaderText = Resources.EvaluationForm_ColStudent;
            dgvGrades.Columns["Student"].ReadOnly = true;
            dgvGrades.Columns["Student"].Width = 400;
            dgvGrades.Columns["Grade"].HeaderText = Resources.EvaluationForm_ColGrade;
            dgvGrades.Columns["Grade"].Width = 120;
        }

        private bool ValidateInputs()
        {
            if (cboCourse.SelectedValue == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.EvaluationForm_NameRequired, ErrorCodesBLL.Business.EvaluationNameRequired),
                    Resources.EvaluationForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private List<GradeBE> CollectGrades()
        {
            var grades = new List<GradeBE>();

            foreach (DataGridViewRow row in dgvGrades.Rows)
            {
                string cellValue = Convert.ToString(row.Cells["Grade"].Value);

                if (string.IsNullOrWhiteSpace(cellValue))
                {
                    continue;
                }

                if (!decimal.TryParse(cellValue, out decimal gradeValue) || gradeValue < 0 || gradeValue > 10)
                {
                    MessageBox.Show(
                        ErrorFormatter.WithCode(Resources.EvaluationForm_GradeOutOfRange, ErrorCodesBLL.Business.GradeOutOfRange),
                        Resources.EvaluationForm_ValidationTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    dgvGrades.CurrentCell = row.Cells["Grade"];
                    return null;
                }

                grades.Add(new GradeBE
                {
                    StudentId = (int)row.Cells["StudentId"].Value,
                    Grade = gradeValue
                });
            }

            return grades;
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            dgvGrades.DataSource = null;
            dgvGrades.Columns.Clear();
        }
        #endregion

        #region Eventos
        private void btnLoadStudents_Click(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            if (dgvGrades.Rows.Count == 0)
            {
                MessageBox.Show(Resources.EvaluationForm_NoStudents,
                    Resources.EvaluationForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<GradeBE> grades = CollectGrades();
            if (grades == null)
            {
                return;
            }

            var evaluation = new EvaluationBE
            {
                CourseId = (int)cboCourse.SelectedValue,
                Name = txtName.Text.Trim(),
                Date = dtpDate.Value.Date,
                CreatedBy = _currentUser.Id
            };

            try
            {
                if (_evaluationBLL.Save(evaluation, grades))
                {
                    MessageBox.Show(Resources.EvaluationForm_Saved, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Resources.EvaluationForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.EvaluationForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
