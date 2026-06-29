using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Filters;
using AzureCSharpRAGAssistant.Api.Mappers;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentsUploadService _documentsUploadService;
        private readonly IDocumentRecordsService _documentRecordsService;

        public DocumentsController(IDocumentsUploadService documentsUploadService, IDocumentRecordsService documentRecordsService)
        {
            _documentsUploadService = documentsUploadService;
            _documentRecordsService = documentRecordsService;
        }

        [HttpPost("upload")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> DocumentUpload([FromForm] DocumentUploadRequest request)
        {
            var document = await _documentsUploadService.UploadDocument(request);
            return Ok(document.ToResponse());
        }

        [HttpGet("getall")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> GetAllDocuments()
        {
            var documents = await _documentRecordsService.GetAllDocumentsAsync();
            var response = documents.Select(x => x.ToResponse());
            return Ok(response);
        }

        [HttpGet("getstatus")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> GetStatusDocuments([FromBody] DocumentStatusQueryObject request)
        {
            var document = await _documentRecordsService.GetByIdAsync(new Guid(request.DocumentId));
            if (document is not null)
            {
                var response = new DocumentStatusResultObject { DocumentId = document.Id.ToString(), Status = document.Status.ToString() };
                return Ok(response);
            }

            return Ok(null);
        }
    }
}
