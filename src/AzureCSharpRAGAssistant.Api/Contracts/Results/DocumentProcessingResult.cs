using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Contracts.Results
{
    public class DocumentProcessingResult
    {
        public bool Succeeded { get; init; }
        public List<Chunk> Chunks { get; init; } = new List<Chunk>();
    }
}