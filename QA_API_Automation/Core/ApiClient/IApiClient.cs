using Flurl.Http;

namespace ENSEK_QA.Core.ApiClient
{
    public interface IApiClient
    {
        Task<IFlurlResponse> PutBuyEnergyAsync(int energyTypeId, int quantityToBuy);
        Task<IFlurlResponse> GetEnergyDetailsAsync();
        Task<IFlurlResponse> PostLoginAsync(string username, string password);
        Task<IFlurlResponse> GetPreviousOrdersAsync();
        Task<IFlurlResponse> PutOrderAsync(string orderId, int energyTypeId, int quantity);
        Task<IFlurlResponse> DeleteOrderIdAsync(string orderId);
        Task<IFlurlResponse> GetOrderByIdAsync(string orderId);
        Task<IFlurlResponse> PostResetTestDataAsync();
    }
}
