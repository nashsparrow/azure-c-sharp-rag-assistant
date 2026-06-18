using Azure;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Controllers;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using AzureCSharpRAGAssistant.Api.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Controllers
{
    public class DocumentsControllerTests
    {

        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<IDocumentProcessingService> _documentProcessingServiceMock;
        private readonly Mock<ILogger<DocumentsController>> _loggerMock;
        private readonly DocumentsController _documentsController;
        private readonly IFormFile _file;

        public DocumentsControllerTests()
        {
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _documentProcessingServiceMock = new Mock<IDocumentProcessingService>();
            _loggerMock = new Mock<ILogger<DocumentsController>>();

            _fileStorageServiceMock
                .Setup(x => x.UploadDocument(It.IsAny<IFormFile>())).ReturnsAsync(Mock.Of<Response<BlobContentInfo>>());
            
            _documentsController = new DocumentsController(
                _loggerMock.Object, 
                _fileStorageServiceMock.Object, 
                _documentProcessingServiceMock.Object);

            _file = TestHelpers.CreateFormFile("test.pdf", "test string");
        }

        [Fact]
        public async void DocumentUpload_ReturnsOk_AndNotProcessesDocument_WhenIndexingIsFalse()
        {
            var request = new DocumentUploadRequest
            {
                File = _file,
                Indexing = false
            };

            var result = await _documentsController.DocumentUpload(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _fileStorageServiceMock.Verify( x => x.UploadDocument(request.File), Times.Once);
            _documentProcessingServiceMock.Verify(x => x.ProcessDocument(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async void DocumentUpload_ReturnsOk_AndProcessesDocument_WhenIndexingIsTrue()
        {
            var request = new DocumentUploadRequest
            {
                File = _file,
                Indexing = true
            };

            var result = await _documentsController.DocumentUpload(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _fileStorageServiceMock.Verify( x => x.UploadDocument(request.File), Times.Once);
            _documentProcessingServiceMock.Verify(x => x.ProcessDocument(It.IsAny<string>()), Times.Once);
        }
    }
}