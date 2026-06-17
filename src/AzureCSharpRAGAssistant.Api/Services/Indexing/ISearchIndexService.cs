using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public interface ISearchIndexService
    {
        Task IndexChunksAsync(IEnumerable<Chunk> chunks);
    }
}