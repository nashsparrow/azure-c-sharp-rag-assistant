using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Services.Storage
{
    public class BlobStorageService : IFileStorageService
    {
        private readonly AzureStorageSettings _storageSettings;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IOptions<AzureStorageSettings> storageSettings, ILogger<BlobStorageService> logger)
        {
            _logger = logger;
            _storageSettings = storageSettings.Value;
        }

        public async Task<List<BlobFileResult>> DownloadAllDocuments(string folderName)
        {
            _logger.LogInformation("Downloading all Documents Started.");
            var blobServiceClient = new BlobServiceClient(_storageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageSettings.ContainerName);

            var files = new List<BlobFileResult>();
            var folderPrefix = $"{folderName}/";

            await foreach (var blobItem in containerClient.GetBlobsAsync(traits: Azure.Storage.Blobs.Models.BlobTraits.None,
             states: Azure.Storage.Blobs.Models.BlobStates.None, prefix: folderPrefix, cancellationToken: default))
            {
                _logger.LogInformation("Downloading File: {FileName}", blobItem.Name);
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var download = await blobClient.DownloadContentAsync();
                _logger.LogInformation("Downloading File: {FileName} Completed", blobItem.Name);

                files.Add(new BlobFileResult { FileName = blobItem.Name, Content = download.Value.Content.ToArray()});
            }

            return files;
        }

        public async Task<BlobFileResult> DownloadDocument(string folderName, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_storageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageSettings.ContainerName);

            var blobPath = $"{folderName}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobPath);

            _logger.LogInformation("Downloading File: {FileName}", fileName);
            var download = await blobClient.DownloadContentAsync();
            _logger.LogInformation("Downloading File: {FileName} Completed", fileName);

            return new BlobFileResult { FileName = fileName, Content = download.Value.Content.ToArray()};
        }

        public async Task<Response<BlobContentInfo>> UploadDocument(IFormFile file)
        {
            _logger.LogInformation("Uploading File: {FileName}", file.FileName);
            ValidateFile(file);

            var blobServiceClient = new BlobServiceClient(_storageSettings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_storageSettings.ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            var blobName = $"documents/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            var result = await blobClient.UploadAsync(stream, overwrite: true);
            _logger.LogInformation("Uploading File: {FileName} Completed", file.FileName);
            return result;
        }

        private void ValidateFile(IFormFile file)
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length == 0)
            {
                _logger.LogError("The uploaded file: {FileName} is empty.", file.FileName);
                throw new InvalidDataException("The uploaded file is empty.");
                
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                _logger.LogError("The uploaded file must have a file name.");
                throw new InvalidDataException("The uploaded file must have a file name.");
            }
        }
    }
}