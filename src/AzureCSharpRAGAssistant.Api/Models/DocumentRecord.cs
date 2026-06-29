namespace AzureCSharpRAGAssistant.Api.Models
{
    public class DocumentRecord
    {
        public Guid Id { get; set; }
        public string ContentHash { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime CreatedUtc { get; set; }
        public bool Indexed { get; set; } = false;
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public Guid JobId { get; set; }
    }

    public enum DocumentStatus
    {
        Pending = 0,
        Uploaded = 1,
        Extracted = 2,
        Cleaned = 3,
        Chunked = 4,
        Embedded = 5,
        Indexed = 6,
        Completed = 7,
        Failed = 8
    }
}