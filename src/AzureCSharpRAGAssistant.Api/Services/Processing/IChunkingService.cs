using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public interface IChunkingService
    {
        List<Chunk>? ChunkText(string fileName, int pageNumber, string text);
    }
}