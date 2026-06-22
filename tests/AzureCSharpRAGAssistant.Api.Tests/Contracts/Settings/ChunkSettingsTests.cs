using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Settings
{
    public class ChunkSettingsTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(10)]
        public void Test_ValidateShouldFail_WhenChunkSize_IsNotInRange(int size)
        {
            var model = new ChunkSettings
            {
                ChunkSize = size
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(ChunkSettings.ChunkSize)));
        }

        [Theory]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public void Test_ValidateShouldPass_WhenChunkSize_IsInRange(int size)
        {
            var model = new ChunkSettings
            {
                ChunkSize = size
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}
