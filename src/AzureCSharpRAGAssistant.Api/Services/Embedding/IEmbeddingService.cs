namespace AzureCSharpRAGAssistant.Api.Services.Embedding
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddings(string chunks);
    }
}