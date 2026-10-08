using BE.Entities;

namespace BE.DTOs
{
    public class LoginResultBE
    {
        #region Propiedades
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public UserBE User { get; set; }

        // Requiere codigo de doble factor (2FA) antes de crear la sesion.
        public bool RequiresTwoFactor { get; set; }
        #endregion
    }
}
