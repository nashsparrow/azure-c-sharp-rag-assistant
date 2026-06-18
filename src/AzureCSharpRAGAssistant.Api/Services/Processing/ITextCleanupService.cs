namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public interface ITextCleanupService
    {
        string CleanupText(string text);
    }
}