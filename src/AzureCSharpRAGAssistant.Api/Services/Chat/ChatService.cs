using System.ClientModel;
using Azure.AI.OpenAI;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Results;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AzureCSharpRAGAssistant.Api.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly AzureOpenAISettings _azureOpenAISettings;

        public ChatService(IOptions<AzureOpenAISettings> openAISettings)
        {
            _azureOpenAISettings = openAISettings.Value;

            var client = new AzureOpenAIClient ( new Uri(_azureOpenAISettings.Endpoint), new ApiKeyCredential(_azureOpenAISettings.ApiKey));

            _chatClient = client.GetChatClient(_azureOpenAISettings.ChatDeployment);    
        }

        public async Task<string> ChatCompletion(string question, ContextBuildResult context)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(_azureOpenAISettings.SystemChatMessage),
                new UserChatMessage(
                    $"""
                    Context: {context.Context}

                    Question: {question}
                    """
                )
            };

            var response = await _chatClient.CompleteChatAsync(messages);

            return response.Value.Content[0].Text;
        }
    }
}