using System.ClientModel;
using Azure.AI.OpenAI;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.ChatHistories;
using AzureCSharpRAGAssistant.Api.Services.ContextBuilder;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AzureCSharpRAGAssistant.Api.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly AzureOpenAISettings _azureOpenAISettings;
        private readonly ISearchIndexService _searchIndexService;
        private readonly IContextBuilderService _contextBuilderService;
        private readonly IChatHistoriesService _chatHistoriesService;

        public ChatService(IOptions<AzureOpenAISettings> openAISettings, ISearchIndexService searchIndexService,
         IContextBuilderService contextBuilderService, IChatHistoriesService chatHistoriesService)
        {
            _azureOpenAISettings = openAISettings.Value;
            _searchIndexService = searchIndexService;
            _contextBuilderService = contextBuilderService;
            _chatHistoriesService = chatHistoriesService;

            var client = new AzureOpenAIClient(new Uri(_azureOpenAISettings.Endpoint), new ApiKeyCredential(_azureOpenAISettings.ApiKey));

            _chatClient = client.GetChatClient(_azureOpenAISettings.ChatDeployment);
        }

        public async Task<string> ChatCompletion(string question, ContextBuildResult context)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_azureOpenAISettings.SystemChatMessage),
                new UserChatMessage(
                    $"""
                    Retrieved context: {context.Context}
                    --------------------

                    Question: {question}

                    Based only on the retrieved context, provide the answer.
                    """
                )
            };

            var response = await _chatClient.CompleteChatAsync(messages);

            return response.Value.Content[0].Text;
        }

        public async Task<string> ChatPipeline(string question)
        {
            var indexResult = await _searchIndexService.SearchChunksAsync(question);
            var context = _contextBuilderService.BuildContext(indexResult);
            var chatResult = await ChatCompletion(question, context);
            await _chatHistoriesService.AddAsync(new ChatHistory
            {
                Question = question,
                Chunks = indexResult,
                Answer = chatResult,
                Context = context.Context
            });

            return chatResult;
        }
    }
}