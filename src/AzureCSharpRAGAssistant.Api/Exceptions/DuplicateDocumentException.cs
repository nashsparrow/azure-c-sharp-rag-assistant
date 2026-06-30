namespace AzureCSharpRAGAssistant.Api.Exceptions
{
    public class DuplicateDocumentException : Exception
    {
        public DuplicateDocumentException(string contentHash)
            : base($"Document content already exists.")
        {
        }
    }
}