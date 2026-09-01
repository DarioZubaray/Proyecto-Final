namespace BE
{
    public class LoginResultBE
    {
        #region Propiedades
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public UserBE User { get; set; }
        #endregion
    }
}
