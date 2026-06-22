using Azure;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Controllers;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Documents;
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
        private readonly Mock<IDocumentsUploadService> _documentsUploadServiceMock;
        private readonly DocumentsController _documentsController;
        private readonly IFormFile _file;

        public DocumentsControllerTests()
        {
            _documentsUploadServiceMock = new Mock<IDocumentsUploadService>();

            _documentsUploadServiceMock
                .Setup(x => x.UploadDocument(It.IsAny<DocumentUploadRequest>()))
                .ReturnsAsync(Mock.Of<Response<DocumentRecord>>());

            _documentsController = new DocumentsController(
                _documentsUploadServiceMock.Object);

            _file = TestHelpers.CreateFormFile("test.pdf", "test string");
        }

        [Fact]
        public async void DocumentUpload_ReturnsOk_WhenIndexingIsFalse()
        {
            var request = new DocumentUploadRequest
            {
                File = _file,
                Indexing = false
            };

            var result = await _documentsController.DocumentUpload(request);
            Assert.IsType<OkObjectResult>(result);

            _documentsUploadServiceMock.Verify(x => x.UploadDocument(request), Times.Once);
        }

        [Fact]
        public async void DocumentUpload_ReturnsOk_WhenIndexingIsTrue()
        {
            var request = new DocumentUploadRequest
            {
                File = _file,
                Indexing = true
            };

            var result = await _documentsController.DocumentUpload(request);
            Assert.IsType<OkObjectResult>(result);

            _documentsUploadServiceMock.Verify(x => x.UploadDocument(request), Times.Once);
        }
    }
}