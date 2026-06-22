using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureStorageSettings
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;
        [Required]
        public string ContainerName { get; set; } = string.Empty;
    }
}