using System;
using System.IO;
using System.Text.Json;

namespace BLL.Helpers
{
    public static class AppPreferencesBLL
    {
        #region Constantes
        private const string DefaultTheme = "System";
        private const string AppFolder = "TrabajoFinal-DarioZubaray";
        private const string FileName = "preferences.json";
        #endregion

        #region Campos
        private static string _filePath = BuildDefaultFilePath();
        private static bool _loaded;
        private static string _language = CultureHelperBLL.DefaultLanguage;
        private static string _theme = DefaultTheme;
        #endregion

        #region Modelo
        private class PreferencesData
        {
            public string Language { get; set; }
            public string Theme { get; set; }
        }
        #endregion

        #region Métodos Públicos
        public static string LastLanguage
        {
            get
            {
                EnsureLoaded();
                return _language;
            }
        }

        public static string LastTheme
        {
            get
            {
                EnsureLoaded();
                return _theme;
            }
        }

        public static void Save(string language, string theme)
        {
            EnsureLoaded();

            _language = NormalizeLanguage(language);
            _theme = string.IsNullOrWhiteSpace(theme) ? DefaultTheme : theme;

            try
            {
                string directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var data = new PreferencesData { Language = _language, Theme = _theme };
                File.WriteAllText(_filePath, JsonSerializer.Serialize(data));
            }
            catch
            {
                // Fallo al persistir; los valores quedan disponibles en memoria.
            }
        }
        #endregion

        #region Métodos Internos
        internal static void SetFilePath(string filePath)
        {
            _filePath = filePath;
            ResetState();
        }

        internal static void Reset()
        {
            _filePath = BuildDefaultFilePath();
            ResetState();
        }
        #endregion

        #region Métodos Privados
        private static void ResetState()
        {
            _loaded = false;
            _language = CultureHelperBLL.DefaultLanguage;
            _theme = DefaultTheme;
        }

        private static string BuildDefaultFilePath()
        {
            string baseDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(baseDirectory, AppFolder, FileName);
        }

        private static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;

            if (!File.Exists(_filePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                PreferencesData data = JsonSerializer.Deserialize<PreferencesData>(json);

                if (data != null)
                {
                    _language = NormalizeLanguage(data.Language);
                    _theme = string.IsNullOrWhiteSpace(data.Theme) ? DefaultTheme : data.Theme;
                }
            }
            catch
            {
                // Archivo corrupto o ilegible: se conservan los valores por defecto.
            }
        }

        private static string NormalizeLanguage(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                return CultureHelperBLL.DefaultLanguage;
            }

            foreach (LanguageItemBLL item in CultureHelperBLL.GetSupportedLanguages())
            {
                if (string.Equals(item.Code, language, StringComparison.OrdinalIgnoreCase))
                {
                    return language;
                }
            }

            return CultureHelperBLL.DefaultLanguage;
        }
        #endregion
    }
}