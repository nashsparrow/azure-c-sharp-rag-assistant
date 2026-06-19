using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.Extensions.Options;
using System.Globalization;

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

            var response = await _searchClient.SearchAsync<SearchDocument>(question, options);

            var chunks = new List<Chunk>();

            await foreach (SearchResult<SearchDocument> result in response.Value.GetResultsAsync())
            {
                if (result.Document is not null)
                {
                    chunks.Add(MapChunk(result.Document));
                }
            }

            return chunks;
        }

        private static Chunk MapChunk(SearchDocument document)
        {
            return new Chunk
            {
                Id = GetString(document, "id"),
                FileId = GetString(document, "fileId"),
                FileName = GetString(document, "fileName"),
                ChunkIndex = GetInt(document, "chunkIndex"),
                PageNumber = GetInt(document, "pageNumber"),
                Content = GetString(document, "content")
            };
        }

        private static string GetString(SearchDocument document, string key)
        {
            return document.TryGetValue(key, out var value) && value is not null
                ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
                : string.Empty;
        }

        private static int GetInt(SearchDocument document, string key)
        {
            if (!document.TryGetValue(key, out var value) || value is null)
            {
                return 0;
            }

            return value switch
            {
                int intValue => intValue,
                long longValue => (int)longValue,
                double doubleValue => (int)doubleValue,
                float floatValue => (int)floatValue,
                _ => Convert.ToInt32(value, CultureInfo.InvariantCulture)
            };
        }
    }
}
