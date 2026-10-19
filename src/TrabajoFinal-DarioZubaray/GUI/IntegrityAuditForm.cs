using System;
using System.Collections.Generic;
using System.Windows.Forms;

using BE.DTOs;
using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;

namespace TrabajoFinal_DarioZubaray
{
    public partial class IntegrityAuditForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly IIntegrityBLL _integrityBLL;
        private readonly IUserBLL _userBLL;
        private List<IntegrityViolationBE> _violations = new List<IntegrityViolationBE>();
        #endregion

        #region Constructor
        public IntegrityAuditForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _integrityBLL = ServiceLocatorBLL.CreateIntegrityBLL();
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
            LoadViolations();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.Integrity_Title;
            lblInfo.Text = Resources.Integrity_InfoLabel;
            btnEdit.Text = Resources.Integrity_EditButton;
            btnAccept.Text = Resources.Integrity_AcceptButton;
            btnRefresh.Text = Resources.Integrity_RefreshButton;
            btnClose.Text = Resources.Integrity_CloseButton;
        }

        private void LoadViolations()
        {
            _violations = _integrityBLL.GetViolations();

            dgvViolations.DataSource = null;
            dgvViolations.DataSource = _violations;
            ConfigureGrid();

            btnEdit.Enabled = _violations.Count > 0;
            btnAccept.Enabled = _violations.Count > 0;
        }

        private void ConfigureGrid()
        {
            if (dgvViolations.Columns.Count == 0)
            {
                return;
            }

            dgvViolations.Columns["TableName"].HeaderText = Resources.Integrity_ColTable;
            dgvViolations.Columns["RowId"].HeaderText = Resources.Integrity_ColRowId;
            dgvViolations.Columns["Description"].HeaderText = Resources.Integrity_ColDescription;
            dgvViolations.Columns["StoredHash"].HeaderText = Resources.Integrity_ColStoredHash;
            dgvViolations.Columns["ComputedHash"].HeaderText = Resources.Integrity_ColComputedHash;

            dgvViolations.Columns["TableName"].Width = 80;
            dgvViolations.Columns["RowId"].Width = 60;
            dgvViolations.Columns["Description"].Width = 140;
            dgvViolations.Columns["StoredHash"].Width = 220;
            dgvViolations.Columns["ComputedHash"].Width = 220;

            dgvViolations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvViolations.MultiSelect = false;
            dgvViolations.ReadOnly = true;
            dgvViolations.AllowUserToAddRows = false;
            dgvViolations.AllowUserToDeleteRows = false;
        }

        private IntegrityViolationBE GetSelectedViolation()
        {
            if (dgvViolations.CurrentRow == null)
            {
                return null;
            }

            return dgvViolations.CurrentRow.DataBoundItem as IntegrityViolationBE;
        }

        private static bool IsEditableViolation(IntegrityViolationBE violation)
        {
            return UserDvhVerifierBLL.TABLE_NAME.Equals(
                violation.TableName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region Eventos
        private void btnEdit_Click(object sender, EventArgs e)
        {
            IntegrityViolationBE violation = GetSelectedViolation();

            if (violation == null)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Integrity_SelectRow,
                    ErrorCodesBLL.Validation.NoSelection));
                return;
            }

            if (!IsEditableViolation(violation))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Integrity_SelectRow,
                    ErrorCodesBLL.Integrity.ResolveFailed));
                return;
            }

            UserBE user = _userBLL.FindById(new UserBE { Id = violation.RowId });

            if (user == null)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Integrity_AcceptFailed,
                    ErrorCodesBLL.Integrity.ResolveFailed));
                LoadViolations();
                return;
            }

            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;

            using (var form = new UserForm(user, theme))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadViolations();
                }
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            IntegrityViolationBE violation = GetSelectedViolation();

            if (violation == null)
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Integrity_SelectRow,
                    ErrorCodesBLL.Validation.NoSelection));
                return;
            }

            string message = string.Format(
                Resources.Integrity_AcceptConfirmMessage, violation.RowId);

            DialogResult result = MessageBox.Show(
                message,
                Resources.Common_Confirm,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            if (_integrityBLL.AcceptAndRegenerate(violation))
            {
                MessageBox.Show(Resources.Integrity_AcceptSuccess);
            }
            else
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Integrity_AcceptFailed,
                    ErrorCodesBLL.Integrity.ResolveFailed));
            }

            LoadViolations();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadViolations();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
