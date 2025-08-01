using ENSEK_QA.Models.Login;
using Flurl.Http;
using Newtonsoft.Json.Linq;

namespace ENSEK_QA.Services.ApiHelper
{
    public static class AuthHelper
    {
        public static string BaseUrl { get; set; } = ApiConfig.BaseUrl;
        public static string LoginEndpoint { get; set; } = "ENSEK/login";

        /// <summary>
        /// Get Bearer token - Login with username and password. Returns the Login response
        /// </summary>
        /// <returns>Login response </returns>
        public static async Task<LoginApiResponse> LoginAndGetTokenAsync(LoginRequest loginRequest)
        {
            var response = await (BaseUrl + LoginEndpoint)
                .WithHeader("accept", "application/json")
                .WithHeader("Content-Type", "application/json")
                .PostJsonAsync(new { username = loginRequest.Username, password = loginRequest.Password });

            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);
            var token = json["access_token"]?.ToString();

            return new LoginApiResponse
            {
                StatusCode = response.StatusCode,
                IsSuccess = token != null,
                IsUnauthorized = response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized,
                ErrorMessage = token == null ? responseBody : null,
                LoginData = new LoginData { AccessToken = token }
            };
        }
    }
}
