using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public interface IDocumentProcessingService
    {
        Task<List<Chunk>> ProcessAllDocuments();

        Task<List<Chunk>> ProcessDocument(string fileName);
    }
}