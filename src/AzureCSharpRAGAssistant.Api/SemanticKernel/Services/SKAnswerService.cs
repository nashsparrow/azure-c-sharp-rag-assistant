
using AzureCSharpRAGAssistant.Api.Helpers;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Factory;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins;
using Microsoft.SemanticKernel;

namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Services
{
    public class SKAnswerService : ISKAnswerService
    {
        private readonly IKernelFactory _kernelFactory;
        private readonly DocumentSearchPlugin _documentSearchPlugin;

        public SKAnswerService(IKernelFactory kernelFactory, DocumentSearchPlugin documentSearchPlugin)
        {
            _kernelFactory = kernelFactory;
            _documentSearchPlugin = documentSearchPlugin;
        }

        public async Task<string> AnswerAsync(string question)
        {
            var kernel = _kernelFactory.CreateKernel(_documentSearchPlugin);
            var prompt = await ResourceHelper.ReadEmbeddedResourceAsync("AzureCSharpRAGAssistant.Api.SemanticKernel.Prompts.assistant.txt");
            var result = await kernel.InvokePromptAsync(
                prompt,
                new KernelArguments
                {
                    ["question"] = question
                }
            );

            return result.ToString();
        }
    }
}