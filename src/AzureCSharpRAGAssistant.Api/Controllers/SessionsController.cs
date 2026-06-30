
using AzureCSharpRAGAssistant.Api.Services.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionsService _sessionsService;
        public SessionsController(ISessionsService sessionsService)
        {
            _sessionsService = sessionsService;
        }

        [HttpGet("getall")]
        public async Task<ActionResult> GetAllSessions()
        {
            var sessions = await _sessionsService.GetAllSessionsAsync();
            return Ok(sessions);
        }
    }
}