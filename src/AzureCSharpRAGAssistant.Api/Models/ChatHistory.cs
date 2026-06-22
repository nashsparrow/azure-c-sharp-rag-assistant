namespace AzureCSharpRAGAssistant.Api.Models
{
    public class ChatHistory
    {
        public Guid Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public List<Chunk> Chunks { get; set; } = new List<Chunk>();
        public bool Testing { get; set; } = false;
        public string TestName { get; set; } = string.Empty;
        public string TestRunName { get; set; } = string.Empty;
        public bool SKPath { get; set; } = false;
    }
}