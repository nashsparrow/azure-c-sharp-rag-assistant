using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class DocumentUploadRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}