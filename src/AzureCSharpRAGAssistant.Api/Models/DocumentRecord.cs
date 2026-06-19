namespace AzureCSharpRAGAssistant.Api.Models
{
    public class DocumentRecord
    {
        public Guid Id { get; set; }
        public int ContentHash { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
        public bool Indexed { get; set; } = false;
    }
}