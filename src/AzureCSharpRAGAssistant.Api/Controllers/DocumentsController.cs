using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Filters;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsUploadService _documentsUploadService;

        public DocumentsController(IDocumentsUploadService documentsUploadService)
        {
            _documentsUploadService = documentsUploadService;
        }

        [HttpPost("upload")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> DocumentUpload([FromForm] DocumentUploadRequest request)
        {
            var document = await _documentsUploadService.UploadDocument(request);
            return Ok(document);
        }
    }
}
