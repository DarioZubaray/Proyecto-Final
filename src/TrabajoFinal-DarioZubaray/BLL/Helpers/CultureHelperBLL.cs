using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace BLL.Helpers
{
    public static class CultureHelperBLL
    {
        #region Propiedades
        private static readonly Dictionary<string, CultureInfo> _cultures =
            new Dictionary<string, CultureInfo>
            {
                { "es", new CultureInfo("es") },
                { "en", new CultureInfo("en") },
                { "pt-BR", new CultureInfo("pt-BR") }
            };

        public const string DefaultLanguage = "es";
        #endregion

        #region Métodos
        public static void SetCulture(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode) || !_cultures.ContainsKey(languageCode))
            {
                languageCode = DefaultLanguage;
            }

            CultureInfo culture = _cultures[languageCode];
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        public static List<LanguageItemBLL> GetSupportedLanguages()
        {
            return new List<LanguageItemBLL>
            {
                new LanguageItemBLL { Code = "es", DisplayName = "Español" },
                new LanguageItemBLL { Code = "en", DisplayName = "English" },
                new LanguageItemBLL { Code = "pt-BR", DisplayName = "Português (Brasil)" }
            };
        }
        #endregion
    }

    public class LanguageItemBLL
    {
        #region Propiedades
        public string Code { get; set; }
        public string DisplayName { get; set; }
        #endregion

        #region Métodos
        public override string ToString()
        {
            return DisplayName;
        }
        #endregion
    }
}
