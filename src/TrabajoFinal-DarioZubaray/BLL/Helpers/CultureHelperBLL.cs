using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace BLL.Helpers
{
    public static class CultureHelperBLL
    {
        private static readonly Dictionary<string, CultureInfo> _cultures =
            new Dictionary<string, CultureInfo>
            {
                { "es", new CultureInfo("es") },
                { "en", new CultureInfo("en") },
                { "pt-BR", new CultureInfo("pt-BR") }
            };

        public const string DefaultLanguage = "es";

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
    }

    public class LanguageItemBLL
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
