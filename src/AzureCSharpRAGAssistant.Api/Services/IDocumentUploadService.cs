using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public interface IDocumentUploadService
    {
        Task<Azure.Response<BlobContentInfo>> UploadDocument(IFormFile file);
    }
}