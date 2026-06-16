using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Services.Storage
{
    public interface IFileStorageService
    {
        Task<BlobFileResult> DownloadDocument(string folderName, string fileName);

        Task<List<BlobFileResult>> DownloadAllDocuments(string folderName);

        Task<Azure.Response<BlobContentInfo>> UploadDocument(IFormFile file);
    }
}