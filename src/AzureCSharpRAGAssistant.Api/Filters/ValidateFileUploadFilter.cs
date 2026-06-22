using System.Security.Cryptography;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Exceptions;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AzureCSharpRAGAssistant.Api.Filters
{
    public class ValidateFileUploadFilter : ActionFilterAttribute
    {
        private readonly ILogger<ValidateFileUploadFilter> _logger;
        private readonly IDocumentRecordsService _documentRecordsService;
        public ValidateFileUploadFilter(ILogger<ValidateFileUploadFilter> logger, IDocumentRecordsService documentRecordsService)
        {
            _logger = logger;
            _documentRecordsService = documentRecordsService;
        }

        public async Task OnActionExecuting(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("request", out var value) && value is DocumentUploadRequest request)
            {
                if (request.File is null)
                {
                    throw new FileUploadValidationException("File Cannot be Null");
                }

                if (request.File.Length == 0)
                {
                    _logger.LogError("The uploaded file: {FileName} is empty.", request.File.FileName);
                    throw new InvalidDataException("The uploaded file is empty.");

                }

                if (string.IsNullOrWhiteSpace(request.File.FileName))
                {
                    _logger.LogError("The uploaded file must have a file name.");
                    throw new InvalidDataException("The uploaded file must have a file name.");
                }
            }

            await next();
        }
    }
}