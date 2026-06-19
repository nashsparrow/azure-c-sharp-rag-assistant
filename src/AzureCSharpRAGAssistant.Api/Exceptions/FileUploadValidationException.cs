namespace AzureCSharpRAGAssistant.Api.Exceptions
{
    public class FileUploadValidationException : Exception
    {
        public FileUploadValidationException(string message) : base(message)
        {
        }
    }
}