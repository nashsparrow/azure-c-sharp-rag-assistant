using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public interface ISearchIndexService
    {
        Task<IndexDocumentsResult> IndexChunksAsync(IEnumerable<Chunk> chunks);
    }
}