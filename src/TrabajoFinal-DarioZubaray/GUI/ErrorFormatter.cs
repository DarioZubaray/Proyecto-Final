using BE.Properties;

namespace TrabajoFinal_DarioZubaray
{
    internal static class ErrorFormatter
    {
        public static string WithCode(string message, string errorCode)
        {
            if (string.IsNullOrEmpty(errorCode))
            {
                return message;
            }

            string code = string.Format(Resources.Error_CodeFormat, errorCode);
            return string.IsNullOrEmpty(message)
                ? code
                : string.Concat(message, " ", code);
        }
    }
}
