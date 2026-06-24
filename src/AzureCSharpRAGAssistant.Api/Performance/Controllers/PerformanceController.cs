using AzureCSharpRAGAssistant.Api.Filters;
using AzureCSharpRAGAssistant.Api.Performance.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureCSharpRAGAssistant.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IEvaluationPipelineService _evaluationPipelineService;

        public PerformanceController(IEvaluationPipelineService evaluationPipelineService)
        {
            _evaluationPipelineService = evaluationPipelineService;
        }

        [HttpPost("evaluation")]
        [ServiceFilter(typeof(ValidateFileUploadFilter))]
        public async Task<ActionResult> RecallEvaluation()
        {
            await _evaluationPipelineService.RunAllEvaluations(runRecallEvaluations: true);
            return Ok("Test Executed");
        }
    }
}
