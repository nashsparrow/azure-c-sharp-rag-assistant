using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Settings
{
    public class AzureSearchSettingsTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenAnyOfTheRequiredSetting_IsEmpty(string settingString)
        {
            TestIfFails_WhenApiKeyIsEmpty(settingString);
            TestIfFails_WhenEndpointIsEmpty(settingString);
            TestIfFails_WhenIndexNameIsEmpty(settingString);
        }

        void TestIfFails_WhenApiKeyIsEmpty(string apiKey)
        {
            var model = new AzureSearchSettings
            {
                ApiKey = apiKey,
                IndexName = "index",
                IntegrationTestIndexName = "testName",
                Top_K = 1,
                Endpoint = "endpoint.invalid"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenIndexNameIsEmpty(string setting)
        {
            var model = new AzureSearchSettings
            {
                ApiKey = "apiKey",
                IndexName = setting,
                IntegrationTestIndexName = "testName",
                Top_K = 1,
                Endpoint = "endpoint.invalid"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenEndpointIsEmpty(string setting)
        {
            var model = new AzureSearchSettings
            {
                ApiKey = "apiKey",
                IndexName = "index",
                IntegrationTestIndexName = "testName",
                Top_K = 1,
                Endpoint = setting
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(600)]
        public void Test_ValidateShouldFail_WhenTopK_IsNotInRange(int topk)
        {
            var model = new AzureSearchSettings
            {
                ApiKey = "apiKey",
                IndexName = "index",
                IntegrationTestIndexName = "testName",
                Top_K = topk,
                Endpoint = "endpoint.invalid"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Fact]
        public void Test_ValidateShouldPass_WhenSettingsArePassable()
        {
            var model = new AzureSearchSettings
            {
                ApiKey = "apiKey",
                IndexName = "index",
                IntegrationTestIndexName = "testName",
                Top_K = 5,
                Endpoint = "endpoint.invalid"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
