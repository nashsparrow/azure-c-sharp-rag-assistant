using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Requests
{
    public class FolderSettingsTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenDocumentsFolder_IsEmpty(string folderName)
        {
            var model = new FolderSettings
            {
                DocumentsFolder = folderName,
                OutputFolder = "output"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(FolderSettings.DocumentsFolder)));
        }

        [Theory]
        [InlineData("doc")]
        [InlineData("documents")]
        [InlineData("d")]
        public void Test_ValidateShouldPass_WhenDocumentsFolder_IsNotEmpty(string folderName)
        {
            var model = new FolderSettings
            {
                DocumentsFolder = folderName,
                OutputFolder = "output"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("")]
        [InlineData("    ")]
        public void Test_ValidateShouldFail_WhenOutputFolder_IsEmpty(string folderName)
        {
            var model = new FolderSettings
            {
                DocumentsFolder = "doc",
                OutputFolder = folderName
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(FolderSettings.OutputFolder)));
        }

        [Theory]
        [InlineData("out")]
        [InlineData("output")]
        [InlineData("o")]
        public void Test_ValidateShouldPass_WhenOutputFolder_IsNotEmpty(string folderName)
        {
            var model = new FolderSettings
            {
                DocumentsFolder = "doc",
                OutputFolder = folderName
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.True(isValid);
        }
    }
}