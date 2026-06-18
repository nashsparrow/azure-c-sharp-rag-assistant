using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexingController : ControllerBase
    {
        private readonly IDocumentProcessingService _documentProcessingService;
        public IndexingController(IDocumentProcessingService documentProcessingService)
        {
            _documentProcessingService = documentProcessingService;
        }

        [HttpPost("run")]
        public async Task<ActionResult> RunIndexing()
        {
            var result = await _documentProcessingService.ProcessAllDocuments();
            return Ok(result);
        }
    }
}