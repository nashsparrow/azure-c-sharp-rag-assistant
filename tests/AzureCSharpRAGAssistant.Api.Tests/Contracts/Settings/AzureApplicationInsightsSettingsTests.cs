using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Settings
{
    public class AzureApplicationInsightsSettingsTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenConnectionString_IsEmpty(string connection)
        {
            var model = new AzureApplicationInsightsSettings
            {
                ConnectionString = connection
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(AzureApplicationInsightsSettings.ConnectionString)));
        }

        [Fact]
        public void Test_ValidateShouldPass_WhenConnectionString_IsNotEmpty()
        {
            var model = new AzureApplicationInsightsSettings
            {
                ConnectionString = "test/connect.invalid"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
