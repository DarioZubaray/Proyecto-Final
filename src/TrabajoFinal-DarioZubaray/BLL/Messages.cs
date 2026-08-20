namespace BLL
{
    public static class Messages
    {
        public const string Auth_RequiredFields = "Usuario y contraseña son obligatorios.";
        public const string Auth_InvalidCredentials = "Usuario o contraseña incorrectos.";
        public const string Auth_UserBlocked = "Usuario bloqueado. Contacte al administrador.";
        public const string Auth_LoginSuccess = "Login exitoso.";
        public const string Auth_MaxRetriesExceeded = "Usuario bloqueado por exceso de intentos fallidos. Contacte al administrador.";
        public const string Auth_RetriesLeft = "Usuario o contraseña incorrectos. Intentos restantes: {0}";
    }
}
