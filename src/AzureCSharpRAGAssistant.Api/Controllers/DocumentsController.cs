using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IDocumentUploadService _documentUploadService;

        public DocumentsController(ILogger<DocumentsController> logger, IDocumentUploadService documentUploadService)
        {
            _logger = logger;
            _documentUploadService = documentUploadService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> DocumentUpload([FromForm] DocumentUploadRequest request)
        {
            Console.WriteLine("uploading");
            var result = await _documentUploadService.UploadDocument(request.File);
            return Ok(result);
        }
    }
}
