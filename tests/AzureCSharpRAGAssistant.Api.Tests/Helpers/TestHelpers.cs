using AzureCSharpRAGAssistant.Api.Contracts;
using Microsoft.AspNetCore.Http;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Text;

namespace AzureCSharpRAGAssistant.Api.Tests.Helpers
{
    public static class TestHelpers
    {
        public static IFormFile CreateFormFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };
        }

        public static BlobFileResult CreatePdf(string fileName)
        {
            var blobFile = new BlobFileResult
            {
                FileName = fileName,
                Content = new byte[] { 1, 2, 3 }
            };

            return blobFile;
        }

        public static byte[] CreateMultiPagePdf(params string[] pageTexts)
        {
            using var stream = new MemoryStream();
            var document = new PdfDocument();

            foreach (var pageText in pageTexts)
            {
                var page = document.AddPage();
                using var graphics = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);

                graphics.DrawString(
                    pageText,
                    font,
                    XBrushes.Black,
                    new XRect(40, 40, page.Width - 80, page.Height - 80),
                    XStringFormats.TopLeft);
            }

            document.Save(stream, false);
            return stream.ToArray();
        }
    }
}