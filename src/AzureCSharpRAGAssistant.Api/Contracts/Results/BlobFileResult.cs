namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class BlobFileResult
    {
        public string FileName { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}