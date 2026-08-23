namespace BE
{
    public class LoginResultDTO
    {
        #region Propiedades
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserBE User { get; set; }
        #endregion
    }
}
