using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IFileStorageService _fileStorageService;

        public IDocumentProcessingService DocumentProcessingService { get; set; }

        public DocumentsController(ILogger<DocumentsController> logger, IFileStorageService fileStorageService,
         IDocumentProcessingService documentProcessingService)
        {
            _logger = logger;
            _fileStorageService = fileStorageService;
            DocumentProcessingService = documentProcessingService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> DocumentUpload([FromForm] DocumentUploadRequest request)
        {
            _logger.LogInformation("Uploading File {FileName} with Indexing: {Indexing}.", request.File.FileName, request.Indexing);
            var result = await _fileStorageService.UploadDocument(request.File);

            if (request.Indexing)
            {
                var res = await DocumentProcessingService.ProcessDocument(request.File.FileName);
            }
            return Ok(result);
        }
    }
}
