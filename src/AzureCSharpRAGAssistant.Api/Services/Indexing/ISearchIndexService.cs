using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public interface ISearchIndexService
    {
        Task<IndexDocumentsResult> IndexChunksAsync(IEnumerable<Chunk> chunks);

        Task<List<Chunk>> SearchChunksAsync(string question, int? topK = null, bool isPerformanceRun = false);
    }
}