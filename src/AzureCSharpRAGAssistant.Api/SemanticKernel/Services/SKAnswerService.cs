
using AzureCSharpRAGAssistant.Api.Helpers;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Factory;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins;
using AzureCSharpRAGAssistant.Api.Services.ChatHistories;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Services
{
    public class SKAnswerService : ISKAnswerService
    {
        private readonly IKernelFactory _kernelFactory;
        private readonly DocumentSearchPlugin _documentSearchPlugin;
        private readonly IChatHistoriesService _chatHistoriesService;

        public SKAnswerService(IKernelFactory kernelFactory, DocumentSearchPlugin documentSearchPlugin, IChatHistoriesService chatHistoriesService)
        {
            _kernelFactory = kernelFactory;
            _documentSearchPlugin = documentSearchPlugin;
            _chatHistoriesService = chatHistoriesService;
        }

        public async Task<string> AnswerAsync(string question)
        {
            var kernel = _kernelFactory.CreateKernel(_documentSearchPlugin);
            var prompt = await ResourceHelper.ReadEmbeddedResourceAsync("AzureCSharpRAGAssistant.Api.SemanticKernel.Prompts.assistant.txt");

            var settings = new OpenAIPromptExecutionSettings
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var result = await kernel.InvokePromptAsync(
                prompt,
                new KernelArguments(settings)
                {
                    ["question"] = question
                }
            );

            var answer = result.ToString();

            await _chatHistoriesService.AddAsync(new ChatHistory
            {
                Question = question,
                Answer = answer,
                Status = "Completed",
                SKPath = true
            });

            return answer;
        }
    }
}