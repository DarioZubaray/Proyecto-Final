namespace BLL
{
    public static class ErrorCodesBLL
    {
        public static class Database
        {
            public const string Unavailable = "DB-001";
            public const string Configuration = "DB-002";
            public const string QueryFailed = "DB-003";
        }

        public static class Auth
        {
            public const string RequiredFields = "AUTH-001";
            public const string InvalidCredentials = "AUTH-002";
            public const string UserBlocked = "AUTH-003";
            public const string MaxRetriesExceeded = "AUTH-004";
            public const string RetryAttemptsLeft = "AUTH-005";
        }

        public static class Validation
        {
            public const string UsernameRequired = "VAL-001";
            public const string PasswordRequired = "VAL-002";
            public const string PasswordsMismatch = "VAL-003";
            public const string NameRequired = "VAL-004";
            public const string NoSelection = "VAL-005";
            public const string LanguageThemeRequired = "VAL-006";
        }

        public static class Business
        {
            public const string RoleHasUsers = "BIZ-001";
            public const string InvalidCurrentPassword = "BIZ-002";
        }

        public static class General
        {
            public const string Unhandled = "GEN-001";
        }
    }
}
