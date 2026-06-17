using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public class SearchIndexManagementService : ISearchIndexManagementService
    {
        private readonly SearchIndexClient _searchIndexClient;
        private readonly AzureSearchSettings _searchSettings;

        public SearchIndexManagementService(IOptions<AzureSearchSettings> searchSettings)
        {
            _searchSettings = searchSettings.Value;
            _searchIndexClient = new SearchIndexClient(
                new Uri(_searchSettings.Endpoint), 
                new Azure.AzureKeyCredential(_searchSettings.ApiKey));
        }

        public async Task EnsureIndexExistsAsync()
        {
            try
            {
                await _searchIndexClient.GetIndexAsync(_searchSettings.IndexName);
                return;
            }
            catch (RequestFailedException ex) when (ex.Status == 404)
            {
            }

            var fields = new List<SearchField>
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SimpleField("fileId", SearchFieldDataType.String) { IsFilterable = true },
                new SearchableField("fileName") { IsFilterable = true, IsSortable = true },
                new SimpleField("chunkIndex", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },
                new SimpleField("pageNumber", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },
                new SearchableField("content"),
                new SimpleField("charCount", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },

                new SearchField("contentVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = 1536,
                    VectorSearchProfileName = "vector-profile"
                }
            };

            var index = new SearchIndex(_searchSettings.IndexName, fields)
            {
                VectorSearch = new VectorSearch
                {
                    Profiles =
                    {
                        new VectorSearchProfile("vector-profile", "hnsw-config")
                    },
                    Algorithms =
                    {
                        new HnswAlgorithmConfiguration("hnsw-config")
                    }
                }
            };

            await _searchIndexClient.CreateIndexAsync(index);
        }
    }
}