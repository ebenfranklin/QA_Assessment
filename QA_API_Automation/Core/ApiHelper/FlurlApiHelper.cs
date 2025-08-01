using Flurl.Http;

namespace ENSEK_QA.Services.ApiHelper
{
    public static class FlurlApiHelper
    {
        public static async Task<IFlurlResponse> SendGetAsync(string baseUrl, string endpoint, string token)
        {
            try
            {
                TestContext.WriteLine($"[Request] GET {baseUrl + endpoint} ");
                var response = await (baseUrl + endpoint)
                    .WithHeader("Authorization", $"Bearer {token}")
                    .WithHeader("Content-Type", "application/json")
                    .GetAsync();
                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
                TestContext.WriteLine($"[Response] Status: {response.StatusCode}  Body: {responseBody}");
                return response;
            }
            catch (Flurl.Http.FlurlHttpException exception)
            {
                var errorBody = await exception.GetResponseStringAsync();
                TestContext.WriteLine($"[FlurlHttpException] Status: {exception.Call.Response?.StatusCode}  Body: {errorBody}");
                return exception.Call?.Response;
            }
        }

        public static async Task<IFlurlResponse> SendPostAsync(string baseUrl, string endpoint, string token, object payload = null)
        {
            try
            {
                TestContext.WriteLine($"[Request] POST {baseUrl + endpoint}");
                IFlurlResponse response;
                if (payload != null)
                {
                    response = await (baseUrl + endpoint)
                        .WithHeader("Authorization", $"Bearer {token}")
                        .WithHeader("Content-Type", "application/json")
                        .PostJsonAsync(payload);
                }
                else
                {
                    response = await (baseUrl + endpoint)
                        .WithHeader("Authorization", $"Bearer {token}")
                        .WithHeader("Content-Type", "application/json")
                        .PostAsync(null);
                }
                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
                TestContext.WriteLine($"[Response] Status: {response.StatusCode}  Body: {responseBody}");
                return response;
            }
            catch (Flurl.Http.FlurlHttpException exception)
            {
                var errorBody = await exception.GetResponseStringAsync();
                TestContext.WriteLine($"[FlurlHttpException] Status: {exception.Call.Response?.StatusCode}  Body: {errorBody}");
                return exception.Call?.Response;
            }
        }

        public static async Task<IFlurlResponse> SendPutAsync(string baseUrl, string endpoint, string token, object payload = null)
        {
            try
            {
                TestContext.WriteLine($"[Request] PUT {baseUrl + endpoint}");
                IFlurlResponse response;
                if (payload != null)
                {
                    response = await (baseUrl + endpoint)
                        .WithHeader("Authorization", $"Bearer {token}")
                        .WithHeader("Content-Type", "application/json")
                        .PutJsonAsync(payload);
                }
                else
                {
                    response = await (baseUrl + endpoint)
                        .WithHeader("Authorization", $"Bearer {token}")
                        .WithHeader("Content-Type", "application/json")
                        .PutAsync(null);
                }
                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
                TestContext.WriteLine($"[Response] Status: {response.StatusCode}  Body: {responseBody}");
                return response;
            }
            catch (Flurl.Http.FlurlHttpException exception)
            {
                var errorBody = await exception.GetResponseStringAsync();
                TestContext.WriteLine($"[FlurlHttpException] Status: {exception.Call.Response?.StatusCode}  Body: {errorBody}");
                return exception.Call?.Response;
            }
        }

        public static async Task<IFlurlResponse> SendDeleteAsync(string baseUrl, string endpoint, string token)
        {
            try
            {
                TestContext.WriteLine($"[Request] DELETE {baseUrl + endpoint} ");
                var response = await (baseUrl + endpoint)
                    .WithHeader("Authorization", $"Bearer {token}")
                    .WithHeader("Content-Type", "application/json")
                    .DeleteAsync();
                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
                TestContext.WriteLine($"[Response] Status: {response.StatusCode}  Body: {responseBody}");
                return response;
            }
            catch (Flurl.Http.FlurlHttpException exception)
            {
                var errorBody = await exception.GetResponseStringAsync();
                TestContext.WriteLine($"[FlurlHttpException] Status: {exception.Call.Response?.StatusCode}  Body: {errorBody}");
                return exception.Call?.Response;
            }
        }
    }
}
