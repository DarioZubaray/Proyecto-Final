using System;
using System.Windows.Forms;
using BE;
using BE.Properties;
using BLL;

namespace TrabajoFinal_DarioZubaray
{
    public partial class PreferencesForm : Form
    {
        private readonly UserBE _user;
        private readonly MainForm _mainForm;
        private readonly IUserBLL _userBLL;

        public PreferencesForm(UserBE user, MainForm mainForm)
        {
            InitializeComponent();
            _user = user;
            _mainForm = mainForm;
            _userBLL = ServiceLocator.CreateUserBLL();
            LoadLanguages();
            ApplyResources();
        }

        private void LoadLanguages()
        {
            cbLanguage.DataSource = CultureHelper.GetSupportedLanguages();
            cbLanguage.DisplayMember = "DisplayName";
            cbLanguage.ValueMember = "Code";
            cbLanguage.SelectedValue = _user.Language ?? CultureHelper.DefaultLanguage;
        }

        #region Private Methods
        private void ApplyResources()
        {
            this.Text = Resources.Preferences_Title;
            groupBox1.Text = Resources.Preferences_Title;
            lblLanguage.Text = Resources.Preferences_LanguageLabel;
            btnSave.Text = Resources.Preferences_SaveButton;
        }
        #endregion

        #region Events
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
                _user.Language = selectedLanguage;
                CultureHelper.SetCulture(selectedLanguage);
                _mainForm.ApplyResources();
                ApplyResources();
                MessageBox.Show(Resources.Preferences_SaveSuccess);
            }
        }
        #endregion
    }
}
