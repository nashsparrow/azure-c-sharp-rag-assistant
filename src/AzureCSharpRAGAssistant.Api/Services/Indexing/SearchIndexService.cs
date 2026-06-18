using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public class SearchIndexService : ISearchIndexService
    {
        private readonly SearchClient _searchClient;
        private readonly AzureSearchSettings _searchSettings;

        private readonly ILogger<SearchIndexService> _logger;

        public SearchIndexService(IOptions<AzureSearchSettings> searchSettings, ILogger<SearchIndexService> logger)
        {
            _logger = logger;
            _searchSettings = searchSettings.Value;
            _searchClient = new SearchClient(new Uri(_searchSettings.Endpoint),
            _searchSettings.IndexName,
            new Azure.AzureKeyCredential(_searchSettings.ApiKey));
        }

        public async Task<IndexDocumentsResult> IndexChunksAsync(IEnumerable<Chunk> chunks)
        {
            _logger.LogInformation("Indexing Started.");
            var result = await _searchClient.UploadDocumentsAsync(chunks);
            _logger.LogInformation("Indexing Completed.");
            return result;
        }

        public async Task<List<Chunk>> SearchChunksAsync(string question)
        {
            var options = new SearchOptions
            {
                Size = 5,
                IncludeTotalCount = true
            };

            options.Select.Add("id");
            options.Select.Add("fileId");
            options.Select.Add("fileName");
            options.Select.Add("chunkIndex");
            options.Select.Add("pageNumber");
            options.Select.Add("content");

            var response = await _searchClient.SearchAsync<Chunk>(question, options);

            var chunks = new List<Chunk>();

            await foreach (SearchResult<Chunk> result in response.Value.GetResultsAsync())
            {
                if (result.Document is not null)
                {
                    chunks.Add(result.Document);
                }
            }

            return chunks;
        }
    }
}