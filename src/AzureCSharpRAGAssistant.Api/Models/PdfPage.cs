namespace AzureCSharpRAGAssistant.Api.Models
{
    public class PdfPage
    {
        public string FileName { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}