using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AzureCSharpRAGAssistant.Api.Filters
{
    public class ValidateFileUploadFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("request", out var value) && value is DocumentUploadRequest request)
            {
                if (request.File is null || request.File.Length == 0)
                {
                    throw new FileUploadValidationException("File Cannot be Null");
                }
            }
        }
    }
}