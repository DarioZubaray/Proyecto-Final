using System;
using System.Windows.Forms;

using BE.Entities;
using BE.Properties;
using BLL.Helpers;
using BLL.Interfaces;

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
            LoadThemes();
            ApplyResources();
            ApplyTheme();
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

        private void LoadThemes()
        {
            cbTheme.DataSource = ThemeHelper.GetSupportedThemes();
            cbTheme.DisplayMember = "DisplayName";
            cbTheme.ValueMember = "Code";
            cbTheme.SelectedValue = _user.Theme ?? ThemeHelper.DefaultTheme;
        }

        private void ApplyResources()
        {
            this.Text = Resources.Preferences_Title;
            groupBox1.Text = Resources.Preferences_GroupBox;
            lblLanguage.Text = Resources.Preferences_LanguageLabel;
            lblTheme.Text = Resources.Preferences_ThemeLabel;
            btnSave.Text = Resources.Preferences_SaveButton;

            var themes = ThemeHelper.GetSupportedThemes();
            cbTheme.DataSource = themes;
            cbTheme.DisplayMember = "DisplayName";
            cbTheme.ValueMember = "Code";
            cbTheme.SelectedValue = _user.Theme ?? ThemeHelper.DefaultTheme;
        }

        private void ApplyTheme()
        {
            ThemeHelper.ApplyTheme(this, _user.Theme ?? ThemeHelper.DefaultTheme);
        }
        #endregion

        #region Eventos
        private void btnSave_Click(object sender, EventArgs e)
        {
            string selectedLanguage = cbLanguage.SelectedValue?.ToString();
            string selectedTheme = cbTheme.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(selectedLanguage) || string.IsNullOrEmpty(selectedTheme))
            {
                MessageBox.Show(ErrorFormatter.WithCode(
                    Resources.Preferences_LanguageThemeRequired,
                    ErrorCodesBLL.Validation.LanguageThemeRequired));
                return;
            }

            bool languageSaved = _userBLL.UpdateLanguage(_user.Id, selectedLanguage);
            bool themeSaved = _userBLL.UpdateTheme(_user.Id, selectedTheme);

            if (languageSaved && themeSaved)
            {
                _session.UpdateLanguage(selectedLanguage);
                _session.UpdateTheme(selectedTheme);
                _mainForm.ApplyResources();
                _mainForm.ApplyTheme();
                ThemeHelper.ApplyThemeToAllOpenForms(selectedTheme);
                ApplyResources();
                ApplyTheme();
                MessageBox.Show(Resources.Preferences_SaveSuccess);
            }
        }
        #endregion
    }
}
