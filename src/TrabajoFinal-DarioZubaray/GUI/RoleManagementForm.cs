using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class RoleManagementForm : Form
    {
        #region Propiedades
        private readonly IRoleBLL _roleBLL;
        private List<PermissionBE> _allPermissions;
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
            RoleBE selectedRole = _roleBLL.FindById(roleId);
            List<int> assignedIds = selectedRole.Permissions.Select(p => p.Id).ToList();

            List<PermissionBE> available = _allPermissions
                .Where(p => !assignedIds.Contains(p.Id) && !p.IsSystem)
                .ToList();
            List<PermissionBE> assigned = selectedRole.Permissions.ToList();

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
                MessageBox.Show(Resources.RoleManagement_NameRequired);
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
                MessageBox.Show(Resources.RoleManagement_SelectRole);
                return;
            }

            IUserBLL userBLL = ServiceLocatorBLL.CreateUserBLL();
            int userCount = userBLL.CountByRoleId(role.Id);
            if (userCount > 0)
            {
                MessageBox.Show(
                    string.Format("No se puede eliminar el rol '{0}'. Tiene {1} usuario(s) asociado(s).", role.Name, userCount),
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
                LoadRoles();
                lbAvailable.DataSource = null;
                lbAssigned.DataSource = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            RoleBE role = GetSelectedRole();
            PermissionBE perm = lbAvailable.SelectedItem as PermissionBE;
            if (role == null || perm == null) return;

            List<int> currentIds = _roleBLL.GetPermissionsByRoleId(role.Id).Select(p => p.Id).ToList();
            currentIds.Add(perm.Id);
            _roleBLL.SavePermissions(role.Id, currentIds);

            LoadPermissionLists(role.Id);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RoleBE role = GetSelectedRole();
            PermissionBE perm = lbAssigned.SelectedItem as PermissionBE;
            if (role == null || perm == null || perm.IsSystem) return;

            List<int> currentIds = _roleBLL.GetPermissionsByRoleId(role.Id).Select(p => p.Id).ToList();
            currentIds.Remove(perm.Id);
            _roleBLL.SavePermissions(role.Id, currentIds);

            LoadPermissionLists(role.Id);
        }
        #endregion
    }
}
