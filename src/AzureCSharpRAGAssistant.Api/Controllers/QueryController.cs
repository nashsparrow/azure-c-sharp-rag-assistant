using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using AzureCSharpRAGAssistant.Api.Services.ContextBuilder;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly ISearchIndexService _searchIndexService;
        private readonly IContextBuilderService _contextBuilderService;
        private readonly IChatService _chatService;

        public QueryController(ISearchIndexService searchIndexService, IContextBuilderService contextBuilderService, IChatService chatService)
        {
            _searchIndexService = searchIndexService;
            _contextBuilderService = contextBuilderService;
            _chatService = chatService;
        }

        [HttpPost("query")]
        public async Task<ActionResult> Query([FromForm] QueryRequest request)
        {
            var result = await _searchIndexService.SearchChunksAsync(request.Question);
            var context = _contextBuilderService.BuildContext(result);
            var chatResult = await _chatService.ChatCompletion(request.Question, context);

            return Ok(chatResult);
        }
    }
}