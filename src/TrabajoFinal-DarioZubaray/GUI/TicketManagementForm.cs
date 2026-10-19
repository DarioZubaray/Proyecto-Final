using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;
using BLL.Services;

namespace TrabajoFinal_DarioZubaray
{
    public partial class TicketManagementForm : Form
    {
        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ITicketBLL _ticketBLL;
        private readonly bool _canManage;
        #endregion

        #region Constructor
        public TicketManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _ticketBLL = ServiceLocatorBLL.CreateTicketBLL();

            SessionManagerBLL session = SessionManagerBLL.GetInstance(user.Id);
            _canManage = UserBLL.IsSysAdmin(user)
                || (session != null && session.HasPermission("FORM_TICKET_MGMT"));

            ApplyResources();
            btnClose.Visible = _canManage;
            LoadTickets();
            ThemeHelper.ApplyTheme(this, _currentUser.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.TicketManagementForm_Title;
            btnNew.Text = Resources.TicketManagementForm_BtnNew;
            btnClose.Text = Resources.TicketManagementForm_BtnClose;
        }

        private void LoadTickets()
        {
            dgvTickets.DataSource = null;
            dgvTickets.DataSource = _canManage
                ? _ticketBLL.FindAll()
                : _ticketBLL.FindByUser(_currentUser.Id);
            ConfigureGrid();
        }

        private void ConfigureGrid()
        {
            if (dgvTickets.Columns.Count == 0)
            {
                return;
            }

            dgvTickets.Columns["Id"].HeaderText = "ID";
            dgvTickets.Columns["UserName"].HeaderText = Resources.TicketManagementForm_ColUser;
            dgvTickets.Columns["Subject"].HeaderText = Resources.TicketManagementForm_ColSubject;
            dgvTickets.Columns["IsClosed"].HeaderText = Resources.TicketManagementForm_ColStatus;
            dgvTickets.Columns["CreatedAt"].HeaderText = Resources.TicketManagementForm_ColCreatedAt;
            dgvTickets.Columns["ClosedAt"].HeaderText = Resources.TicketManagementForm_ColClosedAt;
            dgvTickets.Columns["UserId"].Visible = false;
            dgvTickets.Columns["Description"].Visible = false;
            dgvTickets.Columns["ClosedBy"].Visible = false;
            dgvTickets.Columns["ClosedByName"].Visible = false;
            dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTickets.MultiSelect = false;
            dgvTickets.ReadOnly = true;
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
        }

        private TicketBE SelectedTicket()
        {
            return dgvTickets.CurrentRow != null
                ? dgvTickets.CurrentRow.DataBoundItem as TicketBE
                : null;
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            string theme = _currentUser.Theme ?? ThemeHelper.DefaultTheme;
            using (var form = new TicketForm(_currentUser, theme))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadTickets();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            TicketBE ticket = SelectedTicket();

            if (ticket == null)
            {
                return;
            }

            if (ticket.IsClosed)
            {
                MessageBox.Show(
                    Resources.TicketManagementForm_SelectOpenTicket,
                    this.Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult decision = MessageBox.Show(
                Resources.TicketManagementForm_CloseConfirm,
                this.Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (decision != DialogResult.Yes)
            {
                return;
            }

            if (_ticketBLL.Close(ticket.Id, _currentUser.Id))
            {
                MessageBox.Show(Resources.TicketManagementForm_CloseSuccess, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTickets();
            }
        }

        private void dgvTickets_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value is bool closed && dgvTickets.Columns[e.ColumnIndex].Name == "IsClosed")
            {
                e.Value = closed
                    ? Resources.TicketManagementForm_StatusClosed
                    : Resources.TicketManagementForm_StatusOpen;
                e.FormattingApplied = true;
            }
        }
        #endregion
    }
}
