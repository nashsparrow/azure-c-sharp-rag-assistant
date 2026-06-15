using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public interface IDocumentDownloadService
    {
        Task DownloadDocument();

        Task DownloadAllDocuments();
    }
}