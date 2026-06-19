using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Services;

namespace AzureCSharpRAGAssistant.Api.IntegrationTests.Services.Extraction
{
    public class PdfExtractionIntegrationTests
    {
        private readonly PdfExtractionService _pdfExtractionService;
        public PdfExtractionIntegrationTests()
        {
            _pdfExtractionService = new PdfExtractionService();
        }

        [Fact]
        public void Test_PdfExtraction_Returns_ExactPdfPagesCount_And_ContentRelatedToEachPage()
        {
            var assembly = typeof(PdfExtractionIntegrationTests).Assembly;
            var resourceName = "AzureCSharpRAGAssistant.Api.IntegrationTests.Services.Extraction.TestFiles.test_research_paper.pdf";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            Assert.NotNull(stream);

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var blobFile = new BlobFileResult
            {
                FileName = "sample_paper_loop_unrolling.pdf",
                Content = memoryStream.ToArray()
            };

            var result = _pdfExtractionService.ExtractPdfPages(blobFile);
            var pages = result.ToList();
            Assert.Equal(4, pages.Count());
            Assert.Contains("A Dummy Research-Style Study of Public News Sentence",
                pages[0].Text);
            Assert.Contains("The next extracted line should appear as a separate visual line",
                pages[1].Text);
            Assert.Contains("4. Discussion",
                pages[2].Text);
            Assert.Contains("This document is intentionally artificial",
                pages[3].Text);
        }
    }
}