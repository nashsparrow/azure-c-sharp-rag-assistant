namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureSearchSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
        public string IntegrationTestIndexName { get; set; } = string.Empty;
        public int Top_K { get; set; }
    }
}