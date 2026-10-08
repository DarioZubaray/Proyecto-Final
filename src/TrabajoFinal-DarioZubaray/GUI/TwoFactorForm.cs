using System;
using System.Windows.Forms;

using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    // Dialogo modal que solicita el codigo 2FA. La validacion la realiza la BLL
    // (IAuthBLL.ValidateTwoFactor), delegando en ServiceLocatorBLL.CreateTwoFactorBLL().
    public partial class TwoFactorForm : Form
    {
        #region Propiedades
        // Codigo ingresado por el usuario. Solo es valido si DialogResult == OK.
        public string Code { get; private set; }
        #endregion

        #region Constructor
        public TwoFactorForm()
        {
            InitializeComponent();
            ApplyResources();
        }
        #endregion

        #region Métodos
        private void ApplyResources()
        {
            this.Text = Resources.TwoFactor_Title;
            lblPrompt.Text = Resources.Auth_TwoFactorPrompt;
            btnAccept.Text = Resources.TwoFactor_AcceptButton;
            btnCancel.Text = Resources.TwoFactor_CancelButton;
        }
        #endregion

        #region Eventos
        private void btnAccept_Click(object sender, EventArgs e)
        {
            Code = txtCode.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}
