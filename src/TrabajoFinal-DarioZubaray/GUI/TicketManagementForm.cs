using System;
using System.Collections.Generic;
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
        #region Clases Auxiliares
        private sealed class TicketGridRow
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Subject { get; set; }
            public string Status { get; set; }
            public string CreatedAt { get; set; }
            public string ClosedAt { get; set; }
            public TicketBE Ticket { get; set; }
        }
        #endregion

        #region Propiedades
        private readonly UserBE _currentUser;
        private readonly ITicketBLL _ticketBLL;
        private readonly bool _canCreate;
        private readonly bool _canManage;
        #endregion

        #region Constructor
        public TicketManagementForm(UserBE user)
        {
            InitializeComponent();
            _currentUser = user;
            _ticketBLL = ServiceLocatorBLL.CreateTicketBLL();

            SessionManagerBLL session = SessionManagerBLL.GetInstance(user.Id);
            _canCreate = session != null && session.HasPermission("FORM_TICKET_CREATE");
            _canManage = session != null && session.HasPermission("FORM_TICKET_MGMT");

            ApplyResources();
            btnNew.Visible = _canCreate;
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
            List<TicketBE> tickets = _canManage
                ? _ticketBLL.FindAll()
                : _ticketBLL.FindByUser(_currentUser.Id);

            var rows = new List<TicketGridRow>();

            foreach (TicketBE ticket in tickets)
            {
                rows.Add(new TicketGridRow
                {
                    Id = ticket.Id,
                    UserName = ticket.UserName,
                    Subject = ticket.Subject,
                    Status = ticket.IsClosed
                        ? Resources.TicketManagementForm_StatusClosed
                        : Resources.TicketManagementForm_StatusOpen,
                    CreatedAt = ticket.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClosedAt = ticket.ClosedAt.HasValue
                        ? ticket.ClosedAt.Value.ToString("yyyy-MM-dd HH:mm:ss")
                        : string.Empty,
                    Ticket = ticket
                });
            }

            dgvTickets.AutoGenerateColumns = true;
            dgvTickets.DataSource = null;
            dgvTickets.DataSource = rows;
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
            dgvTickets.Columns["Status"].HeaderText = Resources.TicketManagementForm_ColStatus;
            dgvTickets.Columns["CreatedAt"].HeaderText = Resources.TicketManagementForm_ColCreatedAt;
            dgvTickets.Columns["ClosedAt"].HeaderText = Resources.TicketManagementForm_ColClosedAt;
            dgvTickets.Columns["Ticket"].Visible = false;

            dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTickets.MultiSelect = false;
            dgvTickets.ReadOnly = true;
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
        }

        private TicketBE SelectedTicket()
        {
            if (dgvTickets.CurrentRow == null)
            {
                return null;
            }

            var row = dgvTickets.CurrentRow.DataBoundItem as TicketGridRow;
            return row?.Ticket;
        }
        #endregion

        #region Eventos
        private void btnNew_Click(object sender, EventArgs e)
        {
            if (!_canCreate)
            {
                return;
            }

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
            if (!_canManage)
            {
                return;
            }

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
        #endregion
    }
}
