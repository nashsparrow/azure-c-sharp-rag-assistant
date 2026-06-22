using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureOpenAISettings
    {
        [Required]
        public string Endpoint { get; set; } = string.Empty;
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        [Required]
        public string EmbeddingDeployment { get; set; } = string.Empty;
        [Required]
        public string ChatDeployment { get; set; } = string.Empty;
        [Required]
        [StringLength(maximumLength: 2000, MinimumLength = 10)]
        public string SystemChatMessage { get; set; } = string.Empty;
    }
}