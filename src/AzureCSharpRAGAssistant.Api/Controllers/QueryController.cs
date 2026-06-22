using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly IChatService _chatService;

        public QueryController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("chat")]
        public async Task<ActionResult> Query([FromForm] QueryRequest request)
        {
            var chatResult = await _chatService.ChatPipeline(request.Question);
            return Ok(chatResult);
        }
    }
}