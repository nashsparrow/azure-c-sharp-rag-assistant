using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Contracts.Results
{
    public class ContextBuildResult
    {
        public string Context { get; set; } = string.Empty;
        public List<ReferenceSource> Sources {get; set;} = new List<ReferenceSource>();
    }
}