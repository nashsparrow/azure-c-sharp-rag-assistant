using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Embedding
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddings(string chunks);
    }
}