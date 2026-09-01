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
    public partial class RoleManagementForm : Form
    {
        #region Propiedades
        private readonly IRoleBLL _roleBLL;
        private List<PermissionBE> _allPermissions;
        private RoleBE _selectedRole;
        #endregion

        #region Constructor
        public RoleManagementForm(UserBE user)
        {
            InitializeComponent();
            _roleBLL = ServiceLocatorBLL.CreateRoleBLL();
            ApplyResources();
            LoadAllPermissions();
            LoadRoles();
            ThemeHelper.ApplyTheme(this, user.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.RoleManagement_Title;
            lblRoleName.Text = Resources.RoleManagement_RoleLabel;
            btnCreate.Text = Resources.RoleManagement_Create;
            btnDelete.Text = Resources.RoleManagement_Delete;
            lblAvailable.Text = Resources.RoleManagement_Available;
            lblAssigned.Text = Resources.RoleManagement_Assigned;
            btnAdd.Text = Resources.RoleManagement_Add;
            btnRemove.Text = Resources.RoleManagement_Remove;
            btnSave.Text = Resources.RoleManagement_Save;
        }

        private void LoadRoles()
        {
            List<RoleBE> roles = _roleBLL.FindAll();
            lbRoles.DataSource = null;
            lbRoles.DataSource = roles;
            lbRoles.DisplayMember = "Name";
            lbRoles.ValueMember = "Id";
        }

        private void LoadAllPermissions()
        {
            _allPermissions = _roleBLL.GetAllPermissions();
        }

        private void LoadPermissionLists(int roleId)
        {
            _selectedRole = _roleBLL.FindById(roleId);
            RefreshPermissionLists();
        }

        private void RefreshPermissionLists()
        {
            if (_selectedRole == null)
            {
                lbAvailable.DataSource = null;
                lbAssigned.DataSource = null;
                return;
            }

            List<int> assignedIds = _selectedRole.Permissions.Select(p => p.Id).ToList();

            List<PermissionBE> available = _allPermissions
                .Where(p => !assignedIds.Contains(p.Id) && !p.IsSystem)
                .ToList();
            List<PermissionBE> assigned = _selectedRole.Permissions.ToList();

            lbAvailable.DataSource = null;
            lbAvailable.DataSource = available;
            lbAvailable.DisplayMember = "Label";
            lbAvailable.ValueMember = "Id";

            lbAssigned.DataSource = null;
            lbAssigned.DataSource = assigned;
            lbAssigned.DisplayMember = "Label";
            lbAssigned.ValueMember = "Id";

            UpdateRemoveButton();
        }

        private void UpdateRemoveButton()
        {
            PermissionBE perm = lbAssigned.SelectedItem as PermissionBE;
            btnRemove.Enabled = perm != null && !perm.IsSystem;
        }

        private RoleBE GetSelectedRole()
        {
            return lbRoles.SelectedItem as RoleBE;
        }
        #endregion

        #region Eventos
        private void lbRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            RoleBE role = GetSelectedRole();
            if (role != null)
            {
                LoadPermissionLists(role.Id);
            }
        }

        private void lbAssigned_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRemoveButton();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string roleName = txtRoleName.Text.Trim();
            if (string.IsNullOrEmpty(roleName))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.RoleManagement_NameRequired,
                    ErrorCodesBLL.Validation.NameRequired));
                txtRoleName.Focus();
                return;
            }

            var newRole = new RoleBE { Name = roleName };
            int newId = _roleBLL.Save(newRole);
            newRole.Id = newId;

            txtRoleName.Text = "";
            LoadRoles();
            lbRoles.SelectedItem = newRole;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            RoleBE role = GetSelectedRole();
            if (role == null)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.RoleManagement_SelectRole,
                    ErrorCodesBLL.Validation.NoSelection));
                return;
            }

            IUserBLL userBLL = ServiceLocatorBLL.CreateUserBLL();
            int userCount = userBLL.CountByRoleId(role.Id);
            if (userCount > 0)
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(
                        string.Format(Resources.RoleManagement_RoleInUse, role.Name, userCount),
                        ErrorCodesBLL.Business.RoleHasUsers),
                    Resources.RoleManagement_Delete,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                string.Format(Resources.RoleManagement_ConfirmDelete, role.Name),
                Resources.RoleManagement_Delete,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _roleBLL.Delete(role.Id);
                _selectedRole = null;
                LoadRoles();
                lbAvailable.DataSource = null;
                lbAssigned.DataSource = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PermissionBE perm = lbAvailable.SelectedItem as PermissionBE;
            if (_selectedRole == null || perm == null) return;

            if (!_selectedRole.Permissions.Any(p => p.Id == perm.Id))
            {
                _selectedRole.Permissions.Add(perm);
            }

            RefreshPermissionLists();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            PermissionBE perm = lbAssigned.SelectedItem as PermissionBE;
            if (_selectedRole == null || perm == null || perm.IsSystem) return;

            _selectedRole.Permissions.RemoveAll(p => p.Id == perm.Id);

            RefreshPermissionLists();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_selectedRole == null)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.RoleManagement_NoRole,
                    ErrorCodesBLL.Validation.NoSelection));
                return;
            }

            List<int> assignedIds = _selectedRole.Permissions.Select(p => p.Id).ToList();
            _roleBLL.SavePermissions(_selectedRole.Id, assignedIds);

            MessageBox.Show(Resources.RoleManagement_Saved);
            LoadRoles();
            lbRoles.SelectedItem = _selectedRole;
        }
        #endregion
    }
}
