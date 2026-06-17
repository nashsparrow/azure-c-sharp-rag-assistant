namespace AzureCSharpRAGAssistant.Api.Models
{
    public class PdfPage
    {
        public int PageNumber { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}