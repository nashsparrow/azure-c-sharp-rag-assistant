namespace AzureCSharpRAGAssistant.Api.Models
{
    public class ReferenceSource
    {
        public string FileName { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public double? Score { get; set; }
    }
}