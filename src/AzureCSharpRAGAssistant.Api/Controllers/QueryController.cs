using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Services;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ISKAnswerService _sKAnswerService;

        public QueryController(IChatService chatService, ISKAnswerService sKAnswerService)
        {
            _chatService = chatService;
            _sKAnswerService = sKAnswerService;
        }

        [HttpPost("chat")]
        public async Task<ActionResult> Query([FromForm] QueryRequest request)
        {
            var chatResult = await _chatService.ChatPipeline(request.Question);
            return Ok(
                new
                {
                    question = request.Question,
                    answer = chatResult
                });
        }

        [HttpPost("chat/sk")]
        public async Task<ActionResult> QueryWithSemanticKernel([FromForm] QueryRequest request)
        {
            var chatResult = await _sKAnswerService.AnswerAsync(request.Question);
            return Ok(
                new
                {
                    question = request.Question,
                    answer = chatResult
                }
            );
        }
    }
}