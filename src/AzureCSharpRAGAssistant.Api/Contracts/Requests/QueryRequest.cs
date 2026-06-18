using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts.Requests
{
    public class QueryRequest
    {
        [Required]
        public string Question { get; set; } = string.Empty;
    }
}