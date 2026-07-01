using Azure;
using Azure.Storage.Blobs.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Controllers;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using AzureCSharpRAGAssistant.Api.Services.Sessions;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using AzureCSharpRAGAssistant.Api.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Controllers
{
    public class DocumentsControllerTests
    {
        private readonly Mock<IDocumentsUploadService> _documentsUploadServiceMock;
        private readonly Mock<IDocumentRecordsService> _documentRecordServiceMock;
        private readonly Mock<ISessionsService> _sessionsServiceMock;
        private readonly DocumentsController _documentsController;
        private readonly IFormFile _file;

        public DocumentsControllerTests()
        {
            _documentsUploadServiceMock = new Mock<IDocumentsUploadService>();
            _documentRecordServiceMock = new Mock<IDocumentRecordsService>();
            _sessionsServiceMock = new Mock<ISessionsService>();

            _documentsUploadServiceMock
                .Setup(x => x.UploadDocument(It.IsAny<DocumentUploadRequest>()))
                .ReturnsAsync(new DocumentRecord
                {
                    Id = Guid.NewGuid(),
                    FileName = "test.pdf",
                    CreatedUtc = DateTime.UtcNow,
                    Indexed = false
                });

            _sessionsServiceMock
                .Setup(x => x.HandleSessionAndReturnValidAsync(It.IsAny<string>(), ActivityType.upload, It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, string.Empty));

            _documentsController = new DocumentsController(_documentsUploadServiceMock.Object, _documentRecordServiceMock.Object, _sessionsServiceMock.Object);
            _documentsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

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