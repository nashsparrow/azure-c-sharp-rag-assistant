using System;
using System.ClientModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using AzureCSharpRAGAssistant.Api.Contracts;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace AzureCSharpRAGAssistant.Api.Services.Embedding
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly AzureOpenAISettings _azureOpenAISettings;



        public EmbeddingService(IOptions<AzureOpenAISettings> openAISettings)
        {
            _azureOpenAISettings = openAISettings.Value;

            var azureClient = new AzureOpenAIClient(
                new Uri(_azureOpenAISettings.Endpoint),
                new ApiKeyCredential(_azureOpenAISettings.ApiKey));

            _embeddingClient = azureClient.GetEmbeddingClient(_azureOpenAISettings.EmbeddingDeployment);
        }

        public async Task<float[]> GenerateEmbeddings(string text)
        {
            OpenAIEmbedding embedding = await _embeddingClient.GenerateEmbeddingAsync(text);
            return embedding.ToFloats().ToArray();
        }
    }
}