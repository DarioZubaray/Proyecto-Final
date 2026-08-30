using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class UserForm : Form
    {
        #region Propiedades
        private readonly IUserBLL _userBLL;
        private readonly IRoleBLL _roleBLL;
        private readonly UserBE _user;
        private readonly bool _isNewUser;
        #endregion

        #region Constructor
        public UserForm(string theme)
        {
            InitializeComponent();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _roleBLL = ServiceLocatorBLL.CreateRoleBLL();
            _isNewUser = true;
            _user = new UserBE();
            ApplyResources();
            LoadRoles();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }

        public UserForm(UserBE user, string theme)
        {
            InitializeComponent();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _roleBLL = ServiceLocatorBLL.CreateRoleBLL();
            _isNewUser = false;
            _user = user;
            ApplyResources();
            LoadRoles();
            LoadUserData();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                MessageBox.Show(Resources.UserForm_UserNameRequired);
                txtUserName.Focus();
                return false;
            }

            if (_isNewUser && string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show(Resources.UserForm_PasswordRequired);
                txtPassword.Focus();
                return false;
            }

            return true;
        }

        private void MapUserFromUI()
        {
            _user.UserName = txtUserName.Text.Trim();
            _user.IsActive = chkIsActive.Checked;
            _user.RoleId = (int)cbRole.SelectedValue;

            if (_isNewUser)
            {
                _user.PasswordHash = EncryptionBLL.HashPassword(txtPassword.Text);
                _user.RetriesCount = 0;
                _user.CreatedAt = DateTime.Now;
                _user.Language = "es";
            }

            _user.LastUpdate = DateTime.Now;
        }

        private bool SaveUser()
        {
            return _userBLL.Save(_user);
        }

        private void ApplyResources()
        {
            this.Text = _isNewUser ? Resources.UserForm_NewTitle : Resources.UserForm_EditTitle;
            lblUserName.Text = Resources.UserForm_UserNameLabel;
            lblPassword.Text = Resources.UserForm_PasswordLabel;
            lblIsActive.Text = Resources.UserForm_IsActiveLabel;
            lblRole.Text = Resources.UserForm_RoleLabel;
            btnSave.Text = Resources.UserForm_Save;
            btnCancel.Text = Resources.UserForm_Cancel;
        }

        private void LoadRoles()
        {
            List<RoleBE> roles = _roleBLL.FindAll();

            cbRole.DataSource = roles.Select(r => new { r.Id, r.Name }).ToList();
            cbRole.DisplayMember = "Name";
            cbRole.ValueMember = "Id";
            cbRole.SelectedIndex = 0;
        }

        private void LoadUserData()
        {
            txtUserName.Text = _user.UserName;
            chkIsActive.Checked = _user.IsActive;
            cbRole.SelectedValue = _user.RoleId;

            if (_isNewUser)
            {
                txtPassword.Visible = true;
                lblPassword.Visible = true;
            }
            else
            {
                txtPassword.Visible = false;
                lblPassword.Visible = false;
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

            MapUserFromUI();

            if (SaveUser())
            {
                MessageBox.Show(Resources.UserForm_UserSaved);
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
