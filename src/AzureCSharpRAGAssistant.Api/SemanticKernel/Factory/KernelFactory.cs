using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Factory
{
    public class KernelFactory : IKernelFactory
    {
        private readonly AzureOpenAISettings _azureOpenAISettings;

        public KernelFactory(IOptions<AzureOpenAISettings> azureOpenAISettings)
        {
            _azureOpenAISettings = azureOpenAISettings.Value;
        }

        public Kernel CreateKernel(DocumentSearchPlugin documentSearchPlugin)
        {
            var builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(
                deploymentName: _azureOpenAISettings.ChatDeployment,
                endpoint: _azureOpenAISettings.Endpoint,
                apiKey: _azureOpenAISettings.ApiKey
            );

            builder.Plugins.AddFromObject(documentSearchPlugin, "document_search");

            return builder.Build();
        }
    }
}