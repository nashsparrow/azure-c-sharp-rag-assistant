using AzureCSharpRAGAssistant.Api.Contracts.Results;

namespace AzureCSharpRAGAssistant.Api.Services.Chat
{
    public interface IChatService
    {
        public Task<string> ChatCompletion(string question, ContextBuildResult context);
    }
}