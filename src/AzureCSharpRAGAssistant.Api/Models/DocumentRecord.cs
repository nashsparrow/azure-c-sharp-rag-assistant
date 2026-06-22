namespace AzureCSharpRAGAssistant.Api.Models
{
    public class DocumentRecord
    {
        public Guid Id { get; set; }
        public string ContentHash { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
        public bool Indexed { get; set; } = false;
    }
}