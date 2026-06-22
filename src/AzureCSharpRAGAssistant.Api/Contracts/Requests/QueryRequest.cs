using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts.Requests
{
    public class QueryRequest
    {
        [Required]
        [StringLength(maximumLength: 500, MinimumLength = 100)]
        public string Question { get; set; } = string.Empty;
    }
}