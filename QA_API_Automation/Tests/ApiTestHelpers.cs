using ENSEK_QA.Core.ApiClient;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace ENSEK_QA.Tests
{
    public static class ApiTestHelpers
    {
        /// <summary>
        /// Get quantity, price and unit type for a given energy type by passing the energy ID
        /// </summary>
        /// <param name="energyId"></param>
        /// <returns></returns>
        public static async Task<(int? quantity, double? price, string unitType)> GetEnergyDetailsForEnergyId(EnsekApiClient client, int energyId)
        {
            var response = await client.GetEnergyDetailsAsync();
            var responseBody = await response.ResponseMessage.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);

            foreach (var energyType in json)
            {
                var energy = energyType.Value;
                if (energy["energy_id"].Value<int>() == energyId)
                {
                    int quantity = energy["quantity_of_units"].Value<int>();
                    double price = energy["price_per_unit"].Value<double>();
                    string unitType = energy["unit_type"].Value<string>();
                    return (quantity, price, unitType);
                }
            }
            return (null, null, null);
        }

        /// <summary>
        /// Make a purchase and return the order ID
        /// </summary>
        /// <param name="energyTypeId"></param>
        /// <param name="quantityToBuy"></param>
        /// <returns></returns>
        public static async Task<string> PurchaseAndGetOrderIdAsync(EnsekApiClient client, int energyTypeId, int quantityToBuy)
        {
            var purchaseResponse = await client.PutBuyEnergyAsync(energyTypeId, quantityToBuy);
            purchaseResponse.StatusCode.Should().Be(200);
            var purchaseBody = await purchaseResponse.ResponseMessage.Content.ReadAsStringAsync();
            var purchaseJson = ParseResponseBody(purchaseBody);
            string orderId = ExtractOrderIdFromMessage(purchaseJson["message"]?.ToString());
            orderId.Should().NotBeNullOrEmpty();
            return orderId;
        }

        /// <summary>
        /// Extracts the order ID from the message 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ExtractOrderIdFromMessage(string message)
        {
            var match = Regex.Match(
                message ?? "",
                @"order\s*id\s*is\s*([a-fA-F0-9\-]+)",
                RegexOptions.IgnoreCase
            );
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        /// <summary>
        /// Parses the response body and returns a JObject
        /// </summary>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        public static JObject ParseResponseBody(string responseBody)
        {
            return JObject.Parse(responseBody);
        }

        /// <summary>
        /// Asserts that the energy details in the JSON response match the expected values
        /// </summary>
        /// <param name="json"></param>
        /// <param name="energyType"></param>
        /// <param name="expectedId"></param>
        /// <param name="expectedPrice"> </param>
        /// <param name="expectedQuantity"></param>
        /// <param name="expectedUnit"></param>
        public static void AssertEnergyDetails(JObject json, string energyType, int expectedId, double expectedPrice, int expectedQuantity, string expectedUnit)
        {
            json.Should().ContainKey(energyType);
            var energy = json[energyType];
            energy.Should().NotBeNull();
            energy["energy_id"].Value<int>().Should().Be(expectedId);
            energy["price_per_unit"].Value<double>().Should().Be(expectedPrice);
            energy["quantity_of_units"].Value<int>().Should().Be(expectedQuantity);
            energy["unit_type"].Value<string>().Should().Be(expectedUnit);
        }

        /// <summary>
        /// Checks if an order ID exists in a JArray of orders
        /// </summary>
        /// <param name="ordersJson"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool IsOrderIdPresent(JArray ordersJson, string orderId)
        {
            foreach (var order in ordersJson)
            {
                var idToken = order["order_id"];
                if (idToken != null && idToken.ToString() == orderId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Counts the number of orders created before a given date
        /// </summary>
        /// <param name="orders"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int CountOrdersBeforeDate(JArray orders, DateTime date)
        {
            int count = 0;
            foreach (var order in orders)
            {
                var dateToken = order["created_at"] ?? order["createdAt"] ?? order["date"] ?? order["time"];
                if (dateToken != null)
                {
                    TestContext.WriteLine($"Order date : {dateToken}");
                    if (DateTimeOffset.TryParse(dateToken.ToString(), out var orderDate))
                    {
                        if (orderDate.Date < date.Date)
                        {
                            count++;
                        }
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Asserts that the order details in the JSON response match the expected values
        /// </summary>
        /// <param name="orderJson"></param>
        /// <param name="expectedOrderId"></param>
        /// <param name="expectedQuantity"></param>
        /// <param name="expectedEnergyId"></param>
        public static void AssertOrderDetails(JObject orderJson, string expectedOrderId, int expectedQuantity, int expectedEnergyId)
        {
            orderJson["order_id"]?.ToString().Should().Be(expectedOrderId);
            orderJson["quantity"]?.Value<int>().Should().Be(expectedQuantity);
            orderJson["energy_id"]?.Value<int>().Should().Be(expectedEnergyId);
        }
    }
}
