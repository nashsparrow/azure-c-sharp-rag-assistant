using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;

namespace AzureCSharpRAGAssistant.Api.Services.Storage
{
    public class BlobStorageService : IFileStorageService
    {
        private AzureStorageSettings StorageSettings { get; set; }

        public BlobStorageService(AzureStorageSettings storageSettings)
        {
            StorageSettings = storageSettings;
        }

        public async Task<List<BlobFileResult>> DownloadAllDocuments(string folderName)
        {
            var blobServiceClient = new BlobServiceClient(StorageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageSettings.ContainerName);

            var files = new List<BlobFileResult>();
            var folderPrefix = $"{folderName}/";

            await foreach (var blobItem in containerClient.GetBlobsAsync(traits: Azure.Storage.Blobs.Models.BlobTraits.None,
             states: Azure.Storage.Blobs.Models.BlobStates.None, prefix: folderPrefix, cancellationToken: default))
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var download = await blobClient.DownloadContentAsync();

                files.Add(new BlobFileResult { FileName = blobItem.Name, Content = download.Value.Content.ToArray()});
            }

            return files;
        }

        public async Task<BlobFileResult> DownloadDocument(string folderName, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(StorageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageSettings.ContainerName);

            var blobPath = $"{folderName}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobPath);

            var download = await blobClient.DownloadContentAsync();

            return new BlobFileResult { FileName = fileName, Content = download.Value.Content.ToArray()};
        }

        public async Task<Response<BlobContentInfo>> UploadDocument(IFormFile file)
        {
            ValidateFile(file);

            var blobServiceClient = new BlobServiceClient(StorageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(StorageSettings.ContainerName);

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