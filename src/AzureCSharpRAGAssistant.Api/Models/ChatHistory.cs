namespace AzureCSharpRAGAssistant.Api.Models
{
    public class ChatHistory
    {
        public DateTime CreatedTime { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
    }
}