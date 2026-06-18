using System.Text.RegularExpressions;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class TextCleanupService : ITextCleanupService
    {
        public string CleanupText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Replace("/r", "/n");

            // remove repeated spaces and tabs
            text = Regex.Replace(text, @"[ \t]+", " ");

            // remove blank lines
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            // remove - words
            text = Regex.Replace(text, @"(\w)-\n(\w)", "$1$2");

            // join lines inside paragraphs
            text = Regex.Replace(text, @"(?<!\n)\n(?!\n)", " ");
            
            return text.Trim();
        }
    }
}