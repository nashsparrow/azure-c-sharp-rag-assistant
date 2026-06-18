namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public interface ISearchIndexManagementService
    {
        Task EnsureIndexExistsAsync();
    }
}