using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Exceptions
{
    public class DuplicateDocumentException : Exception
    {
        public DuplicateDocumentException(string contentHash)
            : base($"Document content already exists.")
        {
        }
    }
}