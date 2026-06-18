using AzureCSharpRAGAssistant.Api.Services.Processing;

namespace AzureCSharpRAGAssistant.Api.Tests.Services.Processing
{
    public class TextCleanupServiceTests
    {
        private readonly TextCleanupService _textCleanupService;

        public TextCleanupServiceTests()
        {
            _textCleanupService = new TextCleanupService();
        }

        [Fact]
        public void Test_CleanupText_EmptyString_Retruns_Null()
        {
            var result = _textCleanupService.CleanupText("");
            Assert.Null(result);
        }

        [Fact]
        public void Test_CleanupText_EmptySpaceString_Retruns_Null()
        {
            var result = _textCleanupService.CleanupText(" ");
            Assert.Null(result);
        }

        [Fact]
        public void Test_CleanupText_EmptyMultipleSpaceString_Retruns_Null()
        {
            var result = _textCleanupService.CleanupText("        ");
            Assert.Null(result);
        }

        [Fact]
        public void Test_CleanupText_MultipleSpaceString_Retruns_Without_MultipleSpaces()
        {
            var result = _textCleanupService.CleanupText("Test String     With multiple    spaces       ");
            Assert.Equal("Test String With multiple spaces", result);
        }

        [Fact]
        public void Test_CleanupText_LeadingSpacesString_Retruns_Without_LeadingSpaces()
        {
            var result = _textCleanupService.CleanupText("      Test String with Leading Spaces");
            Assert.Equal("Test String with Leading Spaces", result);
        }

        [Fact]
        public void Test_CleanupText_TrailingSpacesString_Retruns_Without_TrailingSpaces()
        {
            var result = _textCleanupService.CleanupText("Test String  with Trailing Spaces          ");
            Assert.Equal("Test String with Trailing Spaces", result);
        }

        [Fact]
        public void Test_CleanupText_RepeatedTabString_Retruns_Without_RepeatedTabs()
        {
            var result = _textCleanupService.CleanupText("Test String  with Repeated             Tabs        ");
            Assert.Equal("Test String with Repeated Tabs", result);
        }

        [Fact]
        public void Test_CleanupText_RepeatedBlankLinesString_Retruns_Without_RepeatedBlankLines()
        {
            var result = _textCleanupService.CleanupText("Test String  with Repeated Blank Lines \n \n\n \n        ");
            Assert.Equal("Test String with Repeated Blank Lines", result);
        }

        [Fact]
        public void Test_CleanupText_HypenedAndNewLineString_Retruns_Without_Hypen()
        {
            var result = _textCleanupService.CleanupText("The docu-\nment was processed.");
            Assert.Equal("The document was processed.", result);
        }

        [Fact]
        public void Test_CleanupText_LinesInsideAParagraphString_Retruns_Without_Lines()
        {
            var result = _textCleanupService.CleanupText("This is line one.\nThis is line two.");
            Assert.Equal("This is line one. This is line two.", result);
        }
    }
}