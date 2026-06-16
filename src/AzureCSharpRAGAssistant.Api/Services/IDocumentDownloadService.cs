using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public interface IDocumentDownloadService
    {
        Task<BlobFileResult> DownloadDocument(string folderName, string fileName);

        Task<List<BlobFileResult>> DownloadAllDocuments(string folderName);
    }
}