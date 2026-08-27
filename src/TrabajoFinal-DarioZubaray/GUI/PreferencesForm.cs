using System;
using System.Windows.Forms;

using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class PreferencesForm : Form
    {
        #region Propiedades
        private readonly UserBE _user;
        private readonly MainForm _mainForm;
        private readonly IUserBLL _userBLL;
        private readonly SessionManagerBLL _session;
        #endregion

        #region Constructor
        public PreferencesForm(UserBE user, MainForm mainForm)
        {
            InitializeComponent();
            _user = user;
            _mainForm = mainForm;
            _userBLL = ServiceLocatorBLL.CreateUserBLL();
            _session = SessionManagerBLL.GetInstance(user.Id);
            LoadLanguages();
            ApplyResources();
        }
        #endregion

        #region Métodos
        private void LoadLanguages()
        {
            cbLanguage.DataSource = CultureHelperBLL.GetSupportedLanguages();
            cbLanguage.DisplayMember = "DisplayName";
            cbLanguage.ValueMember = "Code";
            cbLanguage.SelectedValue = _user.Language ?? CultureHelperBLL.DefaultLanguage;
        }

        private void ApplyResources()
        {
            this.Text = Resources.Preferences_Title;
            groupBox1.Text = Resources.Preferences_GroupBox;
            lblLanguage.Text = Resources.Preferences_LanguageLabel;
            btnSave.Text = Resources.Preferences_SaveButton;
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            string selectedLanguage = cbLanguage.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(selectedLanguage))
            {
                return;
            }

            bool saved = _userBLL.UpdateLanguage(_user.Id, selectedLanguage);

            if (saved)
            {
                _session.UpdateLanguage(selectedLanguage);
                _mainForm.ApplyResources();
                ApplyResources();
                MessageBox.Show(Resources.Preferences_SaveSuccess);
            }
        }
        #endregion
    }
}
