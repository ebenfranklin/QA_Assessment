using ENSEK_QA.Core.ApiClient;
using FluentAssertions;

namespace ENSEK_QA.Tests
{
    public abstract class ApiTestBase
    {
        protected EnsekApiClient client;
        protected virtual string BaseUrl => ENSEK_QA.ApiConfig.BaseUrl;
        protected virtual string Username => "test";
        protected virtual string Password => "testing";

        [SetUp ]
        public async Task SetUp()
        {
            client = new EnsekApiClient();
            if (client != null)
            {
                var resetResponse = await client.PostResetTestDataAsync();
                resetResponse.StatusCode.Should().Be(200);
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            if (client != null)
            {
                var resetResponse = await client.PostResetTestDataAsync();
                resetResponse.StatusCode.Should().Be(200);
            }
        }
    }
}
