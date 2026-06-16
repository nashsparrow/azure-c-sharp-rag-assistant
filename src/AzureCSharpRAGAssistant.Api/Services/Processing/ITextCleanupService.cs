using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public interface ITextCleanupService
    {
        string CleanupText(string text);
    }
}