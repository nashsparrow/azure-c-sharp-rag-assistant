using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.ContextBuilder
{
    public interface IContextBuilderService
    {
        ContextBuildResult BuildContext(List<Chunk> chunks);
    }
}