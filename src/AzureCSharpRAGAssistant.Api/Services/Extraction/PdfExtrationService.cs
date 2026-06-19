using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;


namespace AzureCSharpRAGAssistant.Api.Services
{
    public class PdfExtractionService : IPdfExtractionService
    {
        public IEnumerable<PdfPage> ExtractPdfPages(BlobFileResult document)
        {
            using var pdf = PdfDocument.Open(document.Content);
            
            foreach (var page in pdf.GetPages())
            {
                yield return new PdfPage
                {
                    PageNumber = page.Number,
                    Text = ContentOrderTextExtractor.GetText(page, new ContentOrderTextExtractor.Options
                    {
                        ReplaceWhitespaceWithSpace = true,
                        SeparateParagraphsWithDoubleNewline = false,
                        NegativeGapAsWhitespace = true
                    })
                };
            }
        }
    }
}
