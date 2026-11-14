using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

namespace TrabajoFinal_DarioZubaray
{
    public partial class TicketForm : Form
    {
        #region Propiedades
        private readonly ITicketBLL _ticketBLL;
        private readonly UserBE _currentUser;
        #endregion

        #region Constructor
        public TicketForm(UserBE user, string theme)
        {
            InitializeComponent();
            _currentUser = user;
            _ticketBLL = ServiceLocatorBLL.CreateTicketBLL();
            ApplyResources();
            ThemeHelper.ApplyTheme(this, theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.TicketForm_Title;
            lblSubject.Text = Resources.TicketForm_SubjectLabel;
            lblDescription.Text = Resources.TicketForm_DescriptionLabel;
            btnSave.Text = Resources.TicketForm_BtnSave;
            btnCancel.Text = Resources.TicketForm_BtnCancel;
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.TicketForm_SubjectRequired, ErrorCodesBLL.Business.TicketSubjectRequired),
                    Resources.TicketForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSubject.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show(
                    ErrorFormatter.WithCode(Resources.TicketForm_DescriptionRequired, ErrorCodesBLL.Business.TicketDescriptionRequired),
                    Resources.TicketForm_ValidationTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtDescription.Focus();
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

            var ticket = new TicketBE
            {
                UserId = _currentUser.Id,
                Subject = txtSubject.Text.Trim(),
                Description = txtDescription.Text.Trim()
            };

            try
            {
                if (_ticketBLL.Create(ticket))
                {
                    MessageBox.Show(Resources.TicketForm_CreatedSuccess, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, Resources.TicketForm_ValidationTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
