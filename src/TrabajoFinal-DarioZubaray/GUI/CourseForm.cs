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
    public partial class CourseForm : Form
    {
        #region Propiedades
        private readonly ICourseBLL _courseBLL;
        private readonly IClassroomBLL _classroomBLL;
        private readonly IUserBLL _userBLL;
        private readonly CourseBE _course;
        private readonly bool _isNewCourse;
        private readonly UserBE _currentUser;
        private List<UserBE> _availableTeachers;
        private List<UserBE> _assignedTeachers;
        #endregion

        #region Constructor
        public CourseForm(string theme, UserBE user = null)
        {
            InitializeComponent();
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCourse = true;
            _course = new CourseBE();
            _currentUser = user;
            _availableTeachers = new List<UserBE>();
            _assignedTeachers = new List<UserBE>();
            ApplyResources();
            LoadClassrooms();
            LoadTeachers();
            ConfigureButtons();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public CourseForm(CourseBE course, string theme, UserBE user = null)
        {
            InitializeComponent();
            _courseBLL = ServiceLocatorBLL.CreateCourseBLL();
            _classroomBLL = ServiceLocatorBLL.CreateClassroomBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _isNewCourse = false;
            _course = course;
            _currentUser = user;
            _assignedTeachers = new List<UserBE>(course.Teachers ?? new List<UserBE>());
            ApplyResources();
            LoadClassrooms();
            LoadTeachers();
            LoadCourseData();
            ConfigureButtons();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = _isNewCourse ? Resources.CourseForm_NewTitle : Resources.CourseForm_EditTitle;
            lblName.Text = Resources.CourseForm_NameLabel;
            lblDescription.Text = Resources.CourseForm_DescriptionLabel;
            lblStartDate.Text = Resources.CourseForm_StartDateLabel;
            lblEndDate.Text = Resources.CourseForm_EndDateLabel;
            lblStartTime.Text = Resources.CourseForm_StartTimeLabel;
            lblEndTime.Text = Resources.CourseForm_EndTimeLabel;
            lblClassroom.Text = Resources.CourseForm_ClassroomLabel;
            lblTeachers.Text = Resources.CourseForm_TeachersSection;
            lblAvailable.Text = Resources.CourseForm_AvailableTeachersLabel;
            lblAssigned.Text = Resources.CourseForm_AssignedTeachersLabel;
            btnSave.Text = Resources.CourseForm_Save;
            btnCancel.Text = Resources.CourseForm_Cancel;
            btnAddTeacher.Text = Resources.CourseForm_Add;
            btnRemoveTeacher.Text = Resources.CourseForm_Remove;
        }

        private bool ParseHorario(MaskedTextBox masked, out TimeSpan hora)
        {
            hora = TimeSpan.Zero;

            if (!masked.MaskFull)
            {
                MessageBox.Show(Resources.CourseForm_TimeIncomplete, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            string texto = masked.Text.Replace("_", "").Trim();
            string[] partes = texto.Split(':');

            if (partes.Length != 2
                || !int.TryParse(partes[0], out int horas)
                || !int.TryParse(partes[1], out int minutos))
            {
                MessageBox.Show(Resources.CourseForm_TimeInvalidFormat, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            if (horas < 0 || horas > 23)
            {
                MessageBox.Show(Resources.CourseForm_HoursRange, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            if (minutos != 0 && minutos != 15 && minutos != 30 && minutos != 45)
            {
                MessageBox.Show(Resources.CourseForm_MinutesInvalid, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                masked.Focus();
                return false;
            }

            hora = new TimeSpan(horas, minutos, 0);
            return true;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                MessageBox.Show(Resources.CourseForm_NameRequired, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (dtpStartDate.Value >= dtpEndDate.Value)
            {
                MessageBox.Show(Resources.CourseForm_DatesInvalid, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpStartDate.Focus();
                return false;
            }

            if (!ParseHorario(mtbStartTime, out TimeSpan horaInicio))
                return false;

            if (!ParseHorario(mtbEndTime, out TimeSpan horaFin))
                return false;

            if (horaInicio >= horaFin)
            {
                MessageBox.Show(Resources.CourseForm_TimeRangeInvalid, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtbStartTime.Focus();
                return false;
            }

            if (cbClassroom.SelectedValue == null)
            {
                MessageBox.Show(Resources.CourseForm_ClassroomRequired, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbClassroom.Focus();
                return false;
            }

            if (_assignedTeachers.Count == 0)
            {
                MessageBox.Show(Resources.CourseForm_TeacherRequired, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void MapCourseFromUI()
        {
            _course.Name = txtName.Text.Trim();
            _course.Description = txtDescription.Text.Trim();
            _course.StartDate = dtpStartDate.Value;
            _course.EndDate = dtpEndDate.Value;
            _course.ClassroomId = (int)cbClassroom.SelectedValue;
            _course.DayOfWeek = (int)_course.StartDate.DayOfWeek == 0 ? 7 : (int)_course.StartDate.DayOfWeek;

            ParseHorario(mtbStartTime, out TimeSpan hInicio);
            ParseHorario(mtbEndTime, out TimeSpan hFin);
            _course.StartTime = hInicio;
            _course.EndTime = hFin;

            _course.IsActive = true;

            if (_isNewCourse)
            {
                _course.CreatedAt = DateTime.Now;
            }

            _course.LastUpdate = DateTime.Now;
        }

        private bool SaveCourse()
        {
            bool result = _courseBLL.Save(_course);

            if (result)
            {
                List<int> teacherIds = _assignedTeachers.Select(d => d.Id).ToList();
                _courseBLL.SaveTeachers(_course.Id, teacherIds);
            }

            return result;
        }

        private bool ValidateTeacherOverlap()
        {
            if (_assignedTeachers.Count == 0)
            {
                return false;
            }

            List<int> teacherIds = _assignedTeachers.Select(d => d.Id).ToList();
            return _courseBLL.ValidateTeacherOverlap(_course.Id, _course.DayOfWeek, _course.StartTime, _course.EndTime, teacherIds);
        }

        private void LoadClassrooms()
        {
            List<ClassroomBE> classrooms = _classroomBLL.FindAll();

            cbClassroom.DisplayMember = "Name";
            cbClassroom.ValueMember = "Id";
            cbClassroom.DataSource = classrooms;
            if (cbClassroom.Items.Count > 0)
            {
                cbClassroom.SelectedIndex = 0;
            }
        }

        private void LoadTeachers()
        {
            List<UserBE> allUsers = _userBLL.FindAll();
            _availableTeachers = allUsers.Where(u => u.IsActive && u.RoleId == 2).ToList();

            RefreshTeacherLists();
        }

        private void RefreshTeacherLists()
        {
            var assignedTeacherIds = _assignedTeachers.Select(d => d.Id).ToHashSet();
            var available = _availableTeachers.Where(d => !assignedTeacherIds.Contains(d.Id)).ToList();

            lstAvailableTeachers.DataSource = null;
            lstAvailableTeachers.DataSource = available;
            lstAvailableTeachers.DisplayMember = "UserName";
            lstAvailableTeachers.ValueMember = "Id";

            lstAssignedTeachers.DataSource = null;
            lstAssignedTeachers.DataSource = _assignedTeachers;
            lstAssignedTeachers.DisplayMember = "UserName";
            lstAssignedTeachers.ValueMember = "Id";
        }

        private void LoadCourseData()
        {
            txtName.Text = _course.Name;
            txtDescription.Text = _course.Description;
            dtpStartDate.Value = _course.StartDate;
            dtpEndDate.Value = _course.EndDate;

            cbClassroom.SelectedValue = _course.ClassroomId;

            if (_course.StartTime.HasValue)
            {
                mtbStartTime.Text = _course.StartTime.Value.ToString(@"hh\:mm");
            }

            if (_course.EndTime.HasValue)
            {
                mtbEndTime.Text = _course.EndTime.Value.ToString(@"hh\:mm");
            }
        }

        private void ConfigureButtons()
        {
            bool isAdmin = _currentUser != null && _currentUser.RoleId == 1;
            btnToggleActive.Visible = isAdmin && !_isNewCourse;

            if (btnToggleActive.Visible && !_course.IsActive)
            {
                btnToggleActive.Text = Resources.CourseForm_Deactivate;
            }
            else
            {
                btnToggleActive.Text = Resources.CourseForm_Activate;
            }
        }

        private void btnToggleActive_Click(object sender, EventArgs e)
        {
            if (_course == null || _course.Id == 0)
            {
                return;
            }

            bool isActive = _course.IsActive;
            string titulo = isActive ? Resources.CourseForm_DeactivateTitle : Resources.CourseForm_ActivateTitle;
            string confirmMessage = isActive
                ? string.Format(Resources.CourseForm_DeactivateConfirm, _course.Name)
                : string.Format(Resources.CourseForm_ActivateConfirm, _course.Name);

            DialogResult result = MessageBox.Show(confirmMessage, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    bool success;
                    if (isActive)
                    {
                        success = _courseBLL.Delete(_course);
                    }
                    else
                    {
                        _course.IsActive = true;
                        success = _courseBLL.Save(_course);
                    }

                    if (success)
                    {
                        string mensajeExito = isActive ? Resources.CourseForm_DeactivatedSuccess : Resources.CourseForm_ReactivatedSuccess;
                        MessageBox.Show(mensajeExito);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    string errorKey = isActive ? Resources.CourseForm_DeactivateError : Resources.CourseForm_ActivateError;
                    string mensajeError = string.Format(errorKey, ex.Message);
                    MessageBox.Show(mensajeError, Resources.CourseForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                return;
            }

            MapCourseFromUI();

            if (ValidateTeacherOverlap())
            {
                MessageBox.Show(Resources.CourseForm_TeacherOverlap, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (SaveCourse())
                {
                    string mensaje = _isNewCourse ? Resources.CourseForm_CreatedSuccess : Resources.CourseForm_UpdatedSuccess;
                    MessageBox.Show(mensaje);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Resources.CourseForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                string mensajeError = string.Format(Resources.CourseForm_SaveError, ex.Message);
                MessageBox.Show(mensajeError, Resources.CourseForm_ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAddTeacher_Click(object sender, EventArgs e)
        {
            if (lstAvailableTeachers.SelectedItem == null)
            {
                return;
            }

            var teacher = lstAvailableTeachers.SelectedItem as UserBE;
            if (teacher != null)
            {
                _assignedTeachers.Add(teacher);
                RefreshTeacherLists();
            }
        }

        private void btnRemoveTeacher_Click(object sender, EventArgs e)
        {
            if (lstAssignedTeachers.SelectedItem == null)
            {
                return;
            }

            var teacher = lstAssignedTeachers.SelectedItem as UserBE;
            if (teacher != null)
            {
                _assignedTeachers.Remove(teacher);
                RefreshTeacherLists();
            }
        }
        #endregion
    }
}
