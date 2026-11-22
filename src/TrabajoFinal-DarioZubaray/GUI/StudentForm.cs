using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class StudentForm : Form
    {
        #region Propiedades
        private readonly IStudentBLL _studentBLL;
        private readonly string _theme;
        private StudentBE _student;
        #endregion

        #region Constructores
        public StudentForm(string theme)
        {
            InitializeComponent();
            _studentBLL = ServiceLocatorBLL.CreateStudentBLL();
            _theme = theme ?? ThemeHelper.DefaultTheme;
            _student = new StudentBE();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _theme);
        }

        public StudentForm(StudentBE student, string theme)
        {
            InitializeComponent();
            _studentBLL = ServiceLocatorBLL.CreateStudentBLL();
            _theme = theme ?? ThemeHelper.DefaultTheme;
            _student = student;
            ApplyResources();
            LoadStudent();
            ThemeHelper.ApplyTheme(this, _theme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.StudentForm_Title;
            lblFirstName.Text = Resources.StudentForm_FirstNameLabel;
            lblLastName.Text = Resources.StudentForm_LastNameLabel;
            lblDocument.Text = Resources.StudentForm_DocumentLabel;
            lblEmail.Text = Resources.StudentForm_EmailLabel;
            lblPhone.Text = Resources.StudentForm_PhoneLabel;
            btnSave.Text = Resources.StudentForm_BtnSave;
            btnCancel.Text = Resources.StudentForm_BtnCancel;
        }

        private void LoadStudent()
        {
            txtFirstName.Text = _student.FirstName;
            txtLastName.Text = _student.LastName;
            txtDocument.Text = _student.Document;
            txtEmail.Text = _student.Email;
            txtPhone.Text = _student.Phone;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.StudentForm_FirstNameRequired, ErrorCodesBLL.Business.StudentFirstNameRequired),
                    Resources.StudentForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.StudentForm_LastNameRequired, ErrorCodesBLL.Business.StudentLastNameRequired),
                    Resources.StudentForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtLastName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDocument.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.StudentForm_DocumentRequired, ErrorCodesBLL.Business.StudentDocumentRequired),
                    Resources.StudentForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtDocument.Focus();
                return false;
            }

            return true;
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            _student.FirstName = txtFirstName.Text.Trim();
            _student.LastName = txtLastName.Text.Trim();
            _student.Document = txtDocument.Text.Trim();
            _student.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
            _student.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();

            try
            {
                if (_studentBLL.Save(_student))
                {
                    MessageBox.Show(Resources.StudentForm_SaveSuccess, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Resources.StudentForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.StudentForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
