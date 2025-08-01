namespace ENSEK_QA.Models.Login
{
    public class LoginApiResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsUnauthorized { get; set; }
        public string ErrorMessage { get; set; }
        public LoginData LoginData { get; set; }
    }

    public class LoginData
    {
        public string AccessToken { get; set; }
    }
}
