using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Tests.Helpers;

namespace AzureCSharpRAGAssistant.Api.Tests.Services.Extraction
{
    public class PdfExtractionServiceTests
    {
        private readonly PdfExtractionService _pdfExtractionService;
        public PdfExtractionServiceTests()
        {
            _pdfExtractionService = new PdfExtractionService();
        }

        [Fact]
        public void ExtractPdfPages_Returns_ASingePdfPage_ForASinglePagePdf()
        {
            var fileBytes = TestHelpers.CreateMultiPagePdf(["Page 1 Text"]);
            var blobFile = new BlobFileResult
            {
                FileName = "sample.pdf",
                Content = fileBytes
            };
            var result = _pdfExtractionService.ExtractPdfPages(blobFile);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.IsAssignableFrom<IEnumerable<PdfPage>>(result);
            var pages = result.ToList();
            Assert.Equal(1, pages[0].PageNumber);
            Assert.Equal("Page 1 Text", pages[0].Text);
        }

        [Fact]
        public void ExtractPdfPages_Returns_MultiplePdfPages_ForMultiPagePdf()
        {
            var fileBytes = TestHelpers.CreateMultiPagePdf(["Page 1 Text", "Page 2 Text"]);
            var blobFile = new BlobFileResult
            {
                FileName = "sample.pdf",
                Content = fileBytes
            };
            var result = _pdfExtractionService.ExtractPdfPages(blobFile);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.IsAssignableFrom<IEnumerable<PdfPage>>(result);
            var pages = result.ToList();
            Assert.Equal(1, pages[0].PageNumber);
            Assert.Equal("Page 1 Text", pages[0].Text);
            Assert.Equal(2, pages[1].PageNumber);
            Assert.Equal("Page 2 Text", pages[1].Text);
        }
    }
}