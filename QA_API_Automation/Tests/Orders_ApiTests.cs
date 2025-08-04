using Allure.NUnit;
using Allure.NUnit.Attributes;
using FluentAssertions;

namespace ENSEK_QA.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("ENSEK_API_TESTS")]
    [Category("Ensek Energy Orders API tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Orders_ApiTests : ApiTestBase
    {
        

        [TestCase(1, 10, 200, Category = "TC-L002", Description = "Purchase Gas and get Order Id . Check Get order by ID endpoint returns the correct order details")]
        public async Task GetOrderByIdAsync_ValidData_ShouldReturnOrderDetails(int energyTypeId, int quantityToBuy, int expectedStatus)
        {
            var orderId = await ApiTestHelpers.PurchaseAndGetOrderIdAsync(client, energyTypeId, quantityToBuy);

            // Act - Get previous order by ID
            var response = await client.GetOrderByIdAsync(orderId);
            response.StatusCode.Should().Be(expectedStatus);
            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var orderJson = ApiTestHelpers.ParseResponseBody(responseBody);

            // Assert - The returned order should have the correct order ID
            orderJson["id"]?.ToString().Should().Be(orderId);
        }

        [TestCase("122233344ffgggfdddd", 500, Category = "TC-L003", Description = "Invalid Order Id . Check Get order by ID endpoint returns error")]
        public async Task GetOrderByIdAsync_InValidData_ShouldReturnError(string orderId, int expectedStatus)
        {

            // Act - Get Order Id with invalid order ID
            var response = await client.GetOrderByIdAsync(orderId);
            response.StatusCode.Should().Be(expectedStatus);


            // Assert 
            response.StatusCode.Should().Be(expectedStatus);

        }

        [TestCase(1, 10, 200, Category = "TC-L004", Description = "Purchase Gas, delete order by ID, and verify deletion")]
        public async Task DeleteOrderIdAsync_ValidData_ShouldDeleteOrder(int energyTypeId, int quantityToBuy, int expectedStatus)
        {
            // Arrange - Make a purchase and get the order ID
            var orderId = await ApiTestHelpers.PurchaseAndGetOrderIdAsync(client, energyTypeId, quantityToBuy);

            // Act - Delete the order
            var deleteResponse = await client.DeleteOrderIdAsync(orderId);
            deleteResponse.StatusCode.Should().Be(expectedStatus);

            // Assert - Try to get the deleted order -it should not be present
            var getResponse = await client.GetOrderByIdAsync(orderId);
            getResponse.StatusCode.Should().NotBe(expectedStatus, "Deleted order should not be retrievable");
        }

        [TestCase("123244555566555", 500, Category = "TC-L005", Description = "Invalid order id -Delete order id - Should throw error")]
        public async Task DeleteOrderIdAsync_InValidData_ShouldReturnError(string orderId, int expectedStatus)
        {

            // Act - Delete the order
            var deleteResponse = await client.DeleteOrderIdAsync(orderId);

            // Assert - Try to get the deleted order -it should not be present
            deleteResponse.StatusCode.Should().Be(expectedStatus);
        }

        [TestCase(1, 10, 200, Category = "TC-L006", Description = "Purchase Gas, update order by ID, and verify update succeeds")]
        public async Task PutOrderAsync_ValidData_ShouldUpdateOrder(int energyTypeId, int quantityToBuy, int expectedStatus)
        {
            // Arrange - Make a purchase and get the order ID
            var orderId = await ApiTestHelpers.PurchaseAndGetOrderIdAsync(client, energyTypeId, quantityToBuy);

            // Act - Update the order
            var updateResponse = await client.PutOrderAsync(orderId, energyTypeId, quantityToBuy);
            updateResponse.StatusCode.Should().Be(expectedStatus);

            // Assert - Get the updated order and check the order ID is still present
            var getResponse = await client.GetOrderByIdAsync(orderId);
            getResponse.StatusCode.Should().Be(expectedStatus);
            var responseBody = await getResponse.ResponseMessage.Content.ReadAsStringAsync();
            var orderJson = ApiTestHelpers.ParseResponseBody(responseBody);
            ApiTestHelpers.AssertOrderDetails(orderJson, orderId, quantityToBuy, energyTypeId);
        }

        [TestCase("122277477774", 1, 10, 500, Category = "TC-L007", Description = "Update Invalid order by ID, and verify it returns error ")]
        public async Task PutOrderAsync_InValidData_ShouldReturnError(string orderId, int energyTypeId, int quantityToBuy, int expectedStatus)
        {


            // Act - Update the Invalid order Id
            var updateResponse = await client.PutOrderAsync(orderId, energyTypeId, quantityToBuy);

            // Assert - For error response, the order ID should not be found
            updateResponse.StatusCode.Should().Be(expectedStatus);


        }

      
    }
}
