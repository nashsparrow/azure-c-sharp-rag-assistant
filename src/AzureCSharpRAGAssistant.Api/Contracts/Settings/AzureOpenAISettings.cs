using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Validators;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureOpenAISettings
    {
        [Required]
        [NotWhiteSpace]
        public string Endpoint { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        public string ApiKey { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        public string EmbeddingDeployment { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        public string ChatDeployment { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        [StringLength(maximumLength: 2000, MinimumLength = 10)]
        public string SystemChatMessage { get; set; } = string.Empty;
    }
}