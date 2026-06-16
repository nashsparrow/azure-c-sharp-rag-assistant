using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IFileStorageService _fileStorageService;

        public DocumentsController(ILogger<DocumentsController> logger, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> DocumentUpload([FromForm] DocumentUploadRequest request)
        {
            Console.WriteLine("uploading");
            var result = await _fileStorageService.UploadDocument(request.File);
            return Ok(result);
        }
    }
}
