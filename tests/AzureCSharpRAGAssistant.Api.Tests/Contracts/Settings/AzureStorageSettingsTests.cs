using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Settings
{
    public class AzureStorageSettingsTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenConnectionString_IsEmpty(string connection)
        {
            var model = new AzureStorageSettings
            {
                ConnectionString = connection,
                ContainerName = "container"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AzureStorageSettings.ConnectionString)));
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenContainerName_IsEmpty(string containerName)
        {
            var model = new AzureStorageSettings
            {
                ConnectionString = "connection",
                ContainerName = containerName
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AzureStorageSettings.ContainerName)));
        }

        [Fact]
        public void Test_ValidateShouldPass_WhenContainerName_IsNotEmpty_AndConnection_IsNotEmpty()
        {
            var model = new AzureStorageSettings
            {
                ConnectionString = "connection",
                ContainerName = "containerName"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
