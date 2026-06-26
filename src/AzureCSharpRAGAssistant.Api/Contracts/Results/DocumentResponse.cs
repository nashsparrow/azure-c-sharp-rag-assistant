namespace AzureCSharpRAGAssistant.Api.Contracts.Results
{
    public class DocumentResponse
    {
        public Guid Id { get; init; }
        public string FileName { get; init; } = string.Empty;
        public DateTime CreatedUtc { get; init; }
        public bool Indexed { get; init; }
    }
}