using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services
{
    public class DocumentUploadService : IDocumentUploadService
    {
        private AzureStorageSettings _storageSettings { get; set; }

        public DocumentUploadService(IOptions<AzureStorageSettings> storageSettings)
        {
            _storageSettings = storageSettings.Value;
        }

        public async Task<Response<BlobContentInfo>> UploadDocument(IFormFile file)
        {
            ValidateFile(file);

            var blobServiceClient = new BlobServiceClient(_storageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageSettings.ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"documents/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            var result = await blobClient.UploadAsync(stream, overwrite: true);
            return result;
        }

        private static void ValidateFile(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length == 0)
            {
                throw new InvalidDataException("The uploaded file is empty.");
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                throw new InvalidDataException("The uploaded file must have a file name.");
            }
        }
    }
}
