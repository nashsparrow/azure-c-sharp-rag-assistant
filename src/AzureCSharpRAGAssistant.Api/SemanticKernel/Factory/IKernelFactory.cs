using AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins;
using Microsoft.SemanticKernel;

namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Factory
{
    public interface IKernelFactory
    {
        Kernel CreateKernel(DocumentSearchPlugin documentSearchPlugin);
    }
}