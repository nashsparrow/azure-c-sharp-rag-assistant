using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts.Settings
{
    public class ChunkSettings
    {
        [Required]
        [Range(100, 10000, ErrorMessage = "ChunkSettings:ChunkSize must be between 100 and 10000.")]
        public int ChunkSize { get; set; }
    }
}