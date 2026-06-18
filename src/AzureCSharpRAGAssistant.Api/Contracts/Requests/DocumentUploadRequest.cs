namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class DocumentUploadRequest
    {
        public IFormFile File { get; set; } = default!;
        public bool Indexing { get; set; } = false;
    }
}