using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public class SearchIndexService : ISearchIndexService
    {
        private readonly SearchClient _searchClient;
        private readonly AzureSearchSettings _searchSettings;

        public SearchIndexService(IOptions<AzureSearchSettings> searchSettings)
        {
            _searchSettings = searchSettings.Value;
            _searchClient = new SearchClient(new Uri(_searchSettings.Endpoint),
            _searchSettings.IndexName,
            new Azure.AzureKeyCredential(_searchSettings.ApiKey));
        }

        public async Task<IndexDocumentsResult> IndexChunksAsync(IEnumerable<Chunk> chunks)
        {
            var result = await _searchClient.UploadDocumentsAsync(chunks);
            return result;
        }
    }
}