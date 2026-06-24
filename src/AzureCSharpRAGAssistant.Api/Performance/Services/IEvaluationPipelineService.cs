namespace AzureCSharpRAGAssistant.Api.Performance.Services
{
    public interface IEvaluationPipelineService
    {
        Task RunRecallTests();
    }
}