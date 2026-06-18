using AzureCSharpRAGAssistant.Api.Controllers;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Controllers
{
    public class IndexingControllerTests
    {
        private readonly Mock<IDocumentProcessingService> _documentProcessingServiceMock;
        private readonly IndexingController _indexingController;

        
        public IndexingControllerTests()
        {
            _documentProcessingServiceMock = new Mock<IDocumentProcessingService>();

            _documentProcessingServiceMock.Setup(x => x.ProcessAllDocuments()).ReturnsAsync(new List<Chunk>());
            _indexingController = new IndexingController(_documentProcessingServiceMock.Object);
        }

        [Fact]
        public async Task Test_IndexingController_RunIndexing_ReturnsOK()
        {
            var result = await _indexingController.RunIndexing();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            _documentProcessingServiceMock.Verify(x => x.ProcessAllDocuments(), Times.Once);
        }
    }
}