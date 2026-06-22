using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureSearchSettings
    {
        [Required]
        public string Endpoint { get; set; } = string.Empty;
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        [Required]
        public string IndexName { get; set; } = string.Empty;
        public string IntegrationTestIndexName { get; set; } = string.Empty;
        [Required]
        [Range(1, 10, ErrorMessage = "AzureSearchSettings:Top_K must be between 1 and 10.")]
        public int Top_K { get; set; }
    }
}