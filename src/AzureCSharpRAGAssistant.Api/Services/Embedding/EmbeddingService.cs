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
        private EmbeddingClient EmbeddingClient { get; set; }
        private AzureOpenAISettings AzureOpenAISettings { get; set; }



        public EmbeddingService(IOptions<AzureOpenAISettings> openAISettings)
        {
            AzureOpenAISettings = openAISettings.Value;

            var azureClient = new AzureOpenAIClient(
                new Uri(AzureOpenAISettings.Endpoint),
                new ApiKeyCredential(AzureOpenAISettings.ApiKey));

            EmbeddingClient = azureClient.GetEmbeddingClient(AzureOpenAISettings.EmbeddingDeployment);
        }

        public async Task<float[]> GenerateEmbeddings(string text)
        {
            OpenAIEmbedding embedding = await EmbeddingClient.GenerateEmbeddingAsync(text);
            return embedding.ToFloats().ToArray();
        }
    }
}