using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ISearchIndexService _searchIndexService;

        public QueryController(ISearchIndexService searchIndexService)
        {
            _searchIndexService = searchIndexService;
        }

        [HttpPost("query")]
        public async Task<ActionResult> Query([FromForm] QueryRequest request)
        {
            var result = await _searchIndexService.SearchChunksAsync(request.Question);
            return Ok(result);
        }
    }
}