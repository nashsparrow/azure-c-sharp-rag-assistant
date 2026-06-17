using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Search.Documents;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public class SearchIndexService : ISearchIndexService
    {
        public SearchClient SearchClient { get; set; }
        public AzureSearchSettings SearchSettings { get; set; }

        public SearchIndexService(IOptions<AzureSearchSettings> searchSettings)
        {
            SearchSettings = searchSettings.Value;
            SearchClient = new SearchClient(new Uri(SearchSettings.Endpoint),
            SearchSettings.IndexName,
            new Azure.AzureKeyCredential(SearchSettings.ApiKey));
        }

        public async Task IndexChunksAsync(IEnumerable<Chunk> chunks)
        {
            await SearchClient.UploadDocumentsAsync(chunks);
        }
    }
}