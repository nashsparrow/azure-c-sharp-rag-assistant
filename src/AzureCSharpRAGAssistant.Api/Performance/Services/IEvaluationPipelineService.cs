namespace AzureCSharpRAGAssistant.Api.Performance.Services
{
    public interface IEvaluationPipelineService
    {
        Task RunAllEvaluations(bool runRecallEvaluations = true);
    }
}