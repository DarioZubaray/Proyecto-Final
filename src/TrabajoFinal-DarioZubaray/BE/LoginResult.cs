namespace BE
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int RetriesLeft { get; set; }
        public UserBE User { get; set; }
    }
}
