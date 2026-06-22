using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Settings
{
    public class AzureOpenAISettingsTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenAnyOfTheRequiredSetting_IsEmpty(string settingString)
        {
            TestIfFails_WhenApiKeyIsEmpty(settingString);
            TestIfFails_WhenChatDeploymentIsEmpty(settingString);
            TestIfFails_WhenEmbeddingDeploymentIsEmpty(settingString);
            TestIfFails_WhenEndpointIsEmpty(settingString);
            TestIfFails_WhenSystemChatMessageIsEmpty(settingString);
        }

        void TestIfFails_WhenApiKeyIsEmpty(string apiKey)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = apiKey,
                ChatDeployment = "chat",
                EmbeddingDeployment = "deploy",
                Endpoint = "endpoint",
                SystemChatMessage = "chat message for the system"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenChatDeploymentIsEmpty(string chat)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = chat,
                EmbeddingDeployment = "deploy",
                Endpoint = "endpoint",
                SystemChatMessage = "chat message for the system"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenEmbeddingDeploymentIsEmpty(string setting)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = "chat",
                EmbeddingDeployment = setting,
                Endpoint = "endpoint",
                SystemChatMessage = "chat message for the system"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenEndpointIsEmpty(string setting)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = "chat",
                EmbeddingDeployment = "embedding",
                Endpoint = setting,
                SystemChatMessage = "chat message for the system"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        void TestIfFails_WhenSystemChatMessageIsEmpty(string setting)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = "chat",
                EmbeddingDeployment = "embedding",
                Endpoint = "endpoint",
                SystemChatMessage = setting
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Theory]
        [InlineData("q")]
        [InlineData("this")]
        [InlineData("test")]
        public void Test_ValidateShouldFail_WhenSystemChatMessageIs_IsTooShort(string settingString)
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = "chat",
                EmbeddingDeployment = "deploy",
                Endpoint = "endpoint",
                SystemChatMessage = settingString
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Fact]
        public void Test_ValidateShouldPass_WhenTheSettingsArePassable()
        {
            var model = new AzureOpenAISettings
            {
                ApiKey = "apiKey",
                ChatDeployment = "chat",
                EmbeddingDeployment = "deploy",
                Endpoint = "endpoint",
                SystemChatMessage = "You are a Chat Assistant. Query the Documents."
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
