using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Validators;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureStorageSettings
    {
        [Required]
        [NotWhiteSpace]
        public string ConnectionString { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        public string ContainerName { get; set; } = string.Empty;
    }
}