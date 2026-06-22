using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class DocumentUploadRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
        public bool Indexing { get; set; } = false;
    }
}