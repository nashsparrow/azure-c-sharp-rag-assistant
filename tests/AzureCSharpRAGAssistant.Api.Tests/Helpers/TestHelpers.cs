using Microsoft.AspNetCore.Http;
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
    }
}