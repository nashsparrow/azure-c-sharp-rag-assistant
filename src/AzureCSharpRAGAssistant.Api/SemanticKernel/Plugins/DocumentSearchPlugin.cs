using System.ComponentModel;
using AzureCSharpRAGAssistant.Api.Services.ContextBuilder;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using Microsoft.SemanticKernel;

namespace AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins
{
    public class DocumentSearchPlugin
    {
        private readonly ISearchIndexService _searchIndexService;
        private readonly IContextBuilderService _contextBuilderService;
        public DocumentSearchPlugin(ISearchIndexService searchIndexService, IContextBuilderService contextBuilderService)
        {
            _searchIndexService = searchIndexService;
            _contextBuilderService = contextBuilderService;

        }

        [KernelFunction]
        [Description("Search indexed documents and retrieve context")]
        public async Task<string> SearchDocumentsAsync([Description("User Question")] string question)
        {
            var chunks = await _searchIndexService.SearchChunksAsync(question);
            var contextResult = _contextBuilderService.BuildContext(chunks);

            return contextResult.Context;
        }
    }
}