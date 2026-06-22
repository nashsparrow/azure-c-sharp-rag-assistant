namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Services
{
    public interface ISKAnswerService
    {
        Task<string> AnswerAsync(string question);
    }
}