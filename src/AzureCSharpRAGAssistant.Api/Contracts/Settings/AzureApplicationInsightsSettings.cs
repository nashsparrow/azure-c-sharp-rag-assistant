using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts.Settings
{
    public class AzureApplicationInsightsSettings
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

    }
}