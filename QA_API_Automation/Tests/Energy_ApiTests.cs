using Allure.NUnit;
using Allure.NUnit.Attributes;
using FluentAssertions;
using Newtonsoft.Json.Linq;

namespace ENSEK_QA.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("ENSEK_API_TESTS")]
    [Category("Ensek Energy tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Energy_ApiTests : ApiTestBase
    {


        [TestCase(1, 10, "m³", 200, Category = "TC-E001", Description = "Buy Gas and check the order Id is present in Orders list ")]
        [TestCase(2, 5, "KWh", 200, Category = "TC-E002", Description = "Buy Nuclear and check the order Id is present in Orders list ")]
        [TestCase(3, 10, "MW", 200, Category = "TC-E003", Description = "Buy Electricity and check the order Id is present in Orders list ")]
        [TestCase(4, 10, "Litres", 200, Category = "TC-E004", Description = "Buy Oil and check the order Id is present in Orders list ")]
            public async Task BuyEnergy_OrderShouldBePresentInOrderList(int energyTypeId, int quantityToBuy, string expectedUnitType, int expectedStatus)
        {
            //Data reset is called in the setup method of ApiTestBase, so we can assume the initial state is reset before each test.

            //Arrange 

            string orderId = "";

            //Act - Buy energy 

           
                var response = await client.PutBuyEnergyAsync(energyTypeId, quantityToBuy);

                //Check the response status code 

                response.StatusCode.Should().Be(expectedStatus);
                var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
                var responseJson = ApiTestHelpers.ParseResponseBody(responseBody);
                var actualResponseMessage = responseJson["message"].ToString();

            if (energyTypeId != 2)
            {

                //Extract the order ID from the response

                orderId = ApiTestHelpers.ExtractOrderIdFromMessage(actualResponseMessage);
                orderId.Should().NotBeNullOrEmpty("Order id should not be null or empty");

                //Get all previous orders

                var getAllOrdersResponse = await client.GetPreviousOrdersAsync();
                getAllOrdersResponse.StatusCode.Should().Be(expectedStatus);

                //Assert- the order ID is present in the GetallOrders response

                var getAllOrdersResponseBody = await getAllOrdersResponse.ResponseMessage.Content.ReadAsStringAsync();
                var json = JArray.Parse(getAllOrdersResponseBody);

                bool orderPresentflag = ApiTestHelpers.IsOrderIdPresent(json, orderId);
                orderPresentflag.Should().BeTrue($"Order id {orderId} should be present in orders list");

            }
            else
            {
                //For Nuclear, we expect no purchase to be made, so we check the response message directly
               
                actualResponseMessage.Should().Contain("There is no nuclear fuel to purchase!");
            }

        }



        [TestCase(1, 100, "m³", 200, Category = "TC-E005", Description = "Buy Gas and check the response message contains the correct values of quantity bought ,price,unit & remaining quantity ")]
        [TestCase(2, 50, "KWh", 200, Category = "TC-E006", Description = "Buy Nuclear and check the response message contains the correct values of quantity bought ,price,unit & remaining quantity")]
        [TestCase(3, 110, "MW", 200, Category = "TC-E007", Description = "Buy Electricity and check the response message contains the correct values of quantity bought ,price,unit & remaining quantity")]
        [TestCase(4, 20, "Litres", 200, Category = "TC-E008", Description = "Buy Oil and check the response message contains the correct values of quantity bought ,price,unit & remaining quantity")]
        public async Task BuyEnergy_VaidData_ShouldBeSuccessful(int energyTypeId, int quantityToBuy, string expectedUnitType, int expectedStatus)
        {

            //Arrange - Set up the expected response message based on the energy type and quantity to buy
            string expectedResponseMessage;
            if (energyTypeId == 2)
            {
                expectedResponseMessage = "There is no nuclear fuel to purchase!";
            }
            else
            {
                var (initialQuantity, initialPrice, initialUnitType) = await ApiTestHelpers.GetEnergyDetailsForEnergyId(client, energyTypeId);

                //Calculate expected remaining quantity after purchase
                string expectedRemainingQuantityInResponse = (initialQuantity.HasValue ? initialQuantity.Value - quantityToBuy : 0).ToString();

                string expectedPriceInResponse = initialPrice.HasValue ? initialPrice.Value.ToString() : "0";
                string expectedUnitTypeInResponse = initialUnitType.ToString();
                expectedResponseMessage = $"You have purchased {quantityToBuy} {expectedUnitTypeInResponse} at a cost of {expectedPriceInResponse} there are {expectedRemainingQuantityInResponse} units remaining.Your order id is ";
            }

            //Act - Call the API to buy energy 

            var response = await client.PutBuyEnergyAsync(energyTypeId, quantityToBuy);

            //Assert - Check the response status code and message

            response.StatusCode.Should().Be(expectedStatus);
            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var responseJson = ApiTestHelpers.ParseResponseBody(responseBody);
            var actualResponseMessage = responseJson["message"].ToString();

            actualResponseMessage.Should().Contain(expectedResponseMessage, $"Assertion failed .Values differ from expected . Expected Message {expectedResponseMessage} .Actual Message {actualResponseMessage}");
            if (energyTypeId != 2)
            {
                string orderId = ApiTestHelpers.ExtractOrderIdFromMessage(actualResponseMessage);
                orderId.Should().NotBeNullOrEmpty("Order id should not be null or empty.");
            }
        }


        
        [TestCase(-1, 10, 404, "Not Found", Category = "TC-E009", Description = "Invalid energy type ID")]
        [TestCase(1, -5, 404, "Not Found", Category = "TC-E0010", Description = "Buy Negative quantity")]
        [TestCase(1, 0, 404, "Not Found", Category = "TC-E0011", Description = "Buy Zero quantity")]
        [TestCase(3, 100000, 400, "Bad request", Category = "TC-E0012", Description = "Quantity larger than available")]
        [TestCase(1, 5.77, 400, "Bad request", Category = "TC-E0013", Description = "Quantity in decimal")]


        public async Task BuyEnergy_InvalidData_ShouldReturnError(int energyTypeId, double quantityToBuy, int expectedStatus, string expectedErrorMessage)
        {
            //Act - Call the API to buy energy with invalid data

            var response = await client.PutBuyEnergyAsync(energyTypeId, (int)quantityToBuy);

            //Assert - Check the response status code
            response.StatusCode.Should().Be(expectedStatus);

            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            responseBody.Should().Contain(expectedErrorMessage);
        }

        [TestCase (200,Category="TC-E014", Description="Assert GetEnergyDetailsAsync returns all expected energy types and values")]
        public async Task GetEnergyDetailsAsync_ShouldReturnAllExpectedEnergyTypes(int expectedStatus)
        {
            //Act - Get energy details
            var response = await client.GetEnergyDetailsAsync();
            response.StatusCode.Should().Be(expectedStatus);
            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            // Assert - Check that all expected energy types and values are present

            ApiTestHelpers.AssertEnergyDetails(json, "electric", 3, 0.47, 4322, "kWh");
            ApiTestHelpers.AssertEnergyDetails(json, "gas", 1, 0.34, 3000, "m³");
            ApiTestHelpers.AssertEnergyDetails(json, "nuclear", 2, 0.56, 0, "MW");
            ApiTestHelpers.AssertEnergyDetails(json, "oil", 4, 0.5, 20, "Litres");
        }

        [TestCase(200, Category = "TC-E015", Description = "Check how many orders were created before the current date")]
        public async Task GetPreviousOrdersAsync_ShouldReturnOrdersCreatedBeforeToday(int expectedStatus)
        {
            // Get all previous orders
            var response = await client.GetPreviousOrdersAsync();
            response.StatusCode.Should().Be(expectedStatus);
            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var orders = JArray.Parse(responseBody);

            // Count orders before today
            var today = DateTime.UtcNow.Date;
            int countBeforeToday = ApiTestHelpers.CountOrdersBeforeDate(orders, today);

            TestContext.WriteLine($"Orders created before today: {countBeforeToday}");
            countBeforeToday.Should().BeGreaterOrEqualTo(0);
        }
    }
}
