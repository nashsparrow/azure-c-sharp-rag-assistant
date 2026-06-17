using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndexingController : ControllerBase
    {
        private IDocumentProcessingService DocumentProcessingService { get; set; }
        public IndexingController(IDocumentProcessingService documentProcessingService)
        {
            DocumentProcessingService = documentProcessingService;
        }

        [HttpPost("run")]
        public async Task<ActionResult> RunIndexing()
        {
            Console.WriteLine(" Indexing started..");
            var result = await DocumentProcessingService.ProcessDocuments();
            return Ok(result);
        }
    }
}