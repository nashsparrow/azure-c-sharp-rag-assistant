using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Validators;

namespace AzureCSharpRAGAssistant.Api.Contracts.Settings
{
    public class FolderSettings
    {
        [Required]
        [NotWhiteSpace]
        public string DocumentsFolder { get; set; } = string.Empty;
        [Required]
        [NotWhiteSpace]
        public string OutputFolder { get; set; } = string.Empty;
    }
}