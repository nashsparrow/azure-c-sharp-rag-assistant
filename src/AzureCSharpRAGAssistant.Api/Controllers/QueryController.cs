using AzureCSharpRAGAssistant.Api.Contracts.Requests;
using AzureCSharpRAGAssistant.Api.Exceptions;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Services;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using AzureCSharpRAGAssistant.Api.Services.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueryController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ISKAnswerService _sKAnswerService;
        private readonly ISessionsService _sessionsService;

        public QueryController(IChatService chatService, ISKAnswerService sKAnswerService, ISessionsService sessionsService)
        {
            _chatService = chatService;
            _sKAnswerService = sKAnswerService;
            _sessionsService = sessionsService;
        }

        [HttpPost("chat")]
        public async Task<ActionResult> Query([FromBody] QueryRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var sessionResult = await _sessionsService.HandleSessionAndReturnValidAsync(ipAddress, ActivityType.upload);

            if (!sessionResult.isValid)
            {
                throw new SessionValidationException(sessionResult.reason);
            }

            var chatResult = await _chatService.ChatPipeline(request.Question);
            return Ok(
                new
                {
                    question = request.Question,
                    answer = chatResult
                });
        }

        [HttpPost("chat/sk")]
        public async Task<ActionResult> QueryWithSemanticKernel([FromBody] QueryRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var sessionResult = await _sessionsService.HandleSessionAndReturnValidAsync(ipAddress, ActivityType.upload);

            if (!sessionResult.isValid)
            {
                throw new SessionValidationException(sessionResult.reason);
            }

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