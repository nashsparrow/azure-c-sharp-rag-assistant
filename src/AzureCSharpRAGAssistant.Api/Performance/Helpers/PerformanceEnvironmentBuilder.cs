using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Indexing;

namespace AzureCSharpRAGAssistant.Api.Performance.Helpers
{
    public class PerformanceEnvironmentBuilder
    {
        private readonly ISearchIndexManagementService _searchIndexManagementService;
        public PerformanceEnvironmentBuilder(ISearchIndexManagementService searchIndexManagementService)
        {
            _searchIndexManagementService = searchIndexManagementService;
        }

        public async Task<bool> CreatePerformanceEnvironment()
        {
            await _searchIndexManagementService.EnsureTestIndexExistsAndNewAsync();
            return true;
        }
    }
}