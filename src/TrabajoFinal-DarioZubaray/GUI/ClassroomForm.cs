using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class ClassroomForm : Form
    {
        #region Propiedades
        private readonly IClassroomBLL _classroomBLL;
        private readonly ClassroomBE _classroom;
        private readonly bool _isNewClassroom;
        #endregion

        #region Constructor
        public ClassroomForm(string theme)
        {
            InitializeComponent();
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            _isNewClassroom = true;
            _classroom = new ClassroomBE();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public ClassroomForm(ClassroomBE classroom, string theme)
        {
            InitializeComponent();
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            _isNewClassroom = false;
            _classroom = classroom;
            ApplyResources();
            LoadClassroomData();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = _isNewClassroom ? Resources.ClassroomForm_NewTitle : Resources.ClassroomForm_EditTitle;
            lblName.Text = Resources.ClassroomForm_NameLabel;
            lblCapacity.Text = Resources.ClassroomForm_CapacityLabel;
            btnSave.Text = Resources.ClassroomForm_SaveButton;
            btnCancel.Text = Resources.ClassroomForm_CancelButton;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show(Resources.ClassroomForm_NameRequired, Resources.ClassroomForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (nudCapacity.Value <= 0)
            {
                MessageBox.Show(Resources.ClassroomForm_CapacityInvalid, Resources.ClassroomForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudCapacity.Focus();
                return false;
            }

            return true;
        }

        private void MapClassroomFromUI()
        {
            _classroom.Name = txtName.Text.Trim();
            _classroom.Capacity = (int)nudCapacity.Value;
            _classroom.IsActive = true;

            if (_isNewClassroom)
            {
                _classroom.CreatedAt = DateTime.Now;
            }

            _classroom.LastUpdate = DateTime.Now;
        }

        private bool SaveClassroom()
        {
            return _classroomBLL.Save(_classroom);
        }

        private void LoadClassroomData()
        {
            txtName.Text = _classroom.Name;
            nudCapacity.Value = _classroom.Capacity;
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            MapClassroomFromUI();

            if (SaveClassroom())
            {
                MessageBox.Show(_isNewClassroom ? Resources.ClassroomForm_CreatedSuccess : Resources.ClassroomForm_UpdatedSuccess);
                this.DialogResult = DialogResult.OK;
                this.Close();
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
