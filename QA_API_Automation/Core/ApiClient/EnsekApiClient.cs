using ENSEK_QA.Models.Login;
using ENSEK_QA.Services.ApiHelper;
using ENSEK_QA.Tests;
using Flurl.Http;

namespace ENSEK_QA.Core.ApiClient
{
    public class EnsekApiClient : ApiTestBase, IApiClient
    {
        /// <summary>
        /// Get Bearer token for authentication
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetBearerTokenAsync()
        {
            var loginRequest = new LoginRequest(Username, Password);
            var loginApiResponse = await AuthHelper.LoginAndGetTokenAsync(loginRequest);
            return loginApiResponse.LoginData?.AccessToken;
        }

        /// <summary>
        /// Purchases energy by energy type id and quantity
        /// </summary>
        /// <param name="id">Energy type identifier ;Gas=1 ,Nuclear=2 ,Electricity=3,Oil=4</param>
        /// <param name="quantity">Quantity to purchase</param>
        public async Task<IFlurlResponse> PutBuyEnergyAsync(int energyTypeId, int quantityToBuy)
        {
            var endpoint = $"ENSEK/buy/{energyTypeId}/{quantityToBuy}";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendPutAsync(BaseUrl, endpoint, token);
        }

        /// <summary>
        /// Returns details of all available energy
        /// </summary>
        public async Task<IFlurlResponse> GetEnergyDetailsAsync()
        {
            var endpoint = "ENSEK/energy";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendGetAsync(BaseUrl, endpoint, token);
        }

        /// <summary>
        /// Authenticates and returns the login response
        /// </summary>
        /// <param name="username">username </param>
        /// <param name="password">password </param>
        public async Task<IFlurlResponse> PostLoginAsync(string username, string password)
        {
            var endpoint = "ENSEK/login";
            var token = await GetBearerTokenAsync();
            var payload = new { username, password };
            return await FlurlApiHelper.SendPostAsync(BaseUrl, endpoint, token, payload);
        }

        /// <summary>
        /// Returns details of all previous orders
        /// </summary>
        public async Task<IFlurlResponse> GetPreviousOrdersAsync()
        {
            var endpoint = "ENSEK/orders";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendGetAsync(BaseUrl, endpoint, token);
        }

        /// <summary>
        /// Updates an existing order
        /// </summary>
        /// <param name="orderId">The order ID to update</param>
        public async Task<IFlurlResponse> PutOrderAsync(string orderId, int energyTypeId, int orderQuantity)
        {
            var endpoint = $"ENSEK/orders/{orderId}";
            var token = await GetBearerTokenAsync();
            var payload = new { id = orderId, quantity = orderQuantity, energy_id = energyTypeId };
            return await FlurlApiHelper.SendPutAsync(BaseUrl, endpoint, token, payload);
        }

        /// <summary>
        /// Deletes the order with the specified ID
        /// </summary>
        /// <param name="orderId">The order ID to delete</param>
        public async Task<IFlurlResponse> DeleteOrderIdAsync(string orderId)
        {
            var endpoint = $"ENSEK/orders/{orderId}";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendDeleteAsync(BaseUrl, endpoint, token);
        }

        /// <summary>
        /// Returns details of the single previous order
        /// </summary>
        /// <param name="orderId">The order ID to retrieve</param>
        public async Task<IFlurlResponse> GetOrderByIdAsync(string orderId)
        {
            var endpoint = $"ENSEK/orders/{orderId}";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendGetAsync(BaseUrl, endpoint, token);
        }

        /// <summary>
        /// Resets test data to its initial state
        /// </summary>
        public async Task<IFlurlResponse> PostResetTestDataAsync()
        {
            var endpoint = "ENSEK/reset";
            var token = await GetBearerTokenAsync();
            return await FlurlApiHelper.SendPostAsync(BaseUrl, endpoint, token);
        }
    }
}
