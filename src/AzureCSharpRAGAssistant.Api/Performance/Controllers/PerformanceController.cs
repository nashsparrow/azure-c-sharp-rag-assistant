using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Filters;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IDocumentsUploadService _documentsUploadService;

        public PerformanceController(IDocumentsUploadService documentsUploadService)
        {
            _documentsUploadService = documentsUploadService;
        }

        [HttpPost("recallevaluation")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> RecallEvaluation()
        {
            var document = await _documentsUploadService.UploadDocument(request);
            return Ok(document);
        }
    }
}
