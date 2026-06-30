namespace AzureCSharpRAGAssistant.Api.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public string IP { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int ChatCount { get; set; }
        public int UploadCount { get; set; }
    }
}