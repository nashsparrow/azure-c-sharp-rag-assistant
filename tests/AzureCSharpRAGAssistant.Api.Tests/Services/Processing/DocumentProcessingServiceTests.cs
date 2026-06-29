using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.Extensions.Options;
using Moq;

namespace AzureCSharpRAGAssistant.Api.Tests.Services.Processing
{
    public class DocumentProcessingServiceTests
    {
        private readonly DocumentProcessingService _documentProcessingService;
        private readonly Mock<IPdfExtractionService> _pdfExtractionServiceMock;
        private readonly Mock<ITextCleanupService> _textCleanupServiceMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<IChunkingService> _chunkingServiceMock;
        private readonly Mock<IEmbeddingService> _embeddingServiceMock;
        private readonly Mock<ISearchIndexService> _searchIndexServiceMock;
        private readonly Mock<IDocumentRecordsService> _documentRecordsService;
        private readonly IOptions<FolderSettings> _folderSettings;


        public DocumentProcessingServiceTests()
        {
            _pdfExtractionServiceMock = new Mock<IPdfExtractionService>();
            _textCleanupServiceMock = new Mock<ITextCleanupService>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _chunkingServiceMock = new Mock<IChunkingService>();
            _embeddingServiceMock = new Mock<IEmbeddingService>();
            _searchIndexServiceMock = new Mock<ISearchIndexService>();
            _documentRecordsService = new Mock<IDocumentRecordsService>();

            _folderSettings = Options.Create(new FolderSettings { DocumentsFolder = "test", OutputFolder = "output" });

            _documentProcessingService = new DocumentProcessingService(_pdfExtractionServiceMock.Object, _textCleanupServiceMock.Object,
             _fileStorageServiceMock.Object, _folderSettings, _chunkingServiceMock.Object, _embeddingServiceMock.Object, _searchIndexServiceMock.Object, _documentRecordsService.Object);
        }

        [Fact]
        public async Task Test_ProcessDocument_Behaviour()
        {
            var fileName = "sample.pdf";
            var blobFile = new BlobFileResult
            {
                FileName = fileName,
                Content = new byte[] { 1, 2, 3 }
            };

            var pages = new List<PdfPage>
            {
                new PdfPage { PageNumber = 1, Text = "raw page text" }
            };

            var chunkList = new List<Chunk>
            {
                new Chunk
                {
                    FileName = fileName,
                    PageNumber = 1,
                    Content = "cleaned chunk content"
                }
            };

            _fileStorageServiceMock.Setup(x => x.DownloadDocument(_folderSettings.Value.DocumentsFolder, It.IsAny<string>()))
                .ReturnsAsync(blobFile);

            _pdfExtractionServiceMock.Setup(x => x.ExtractPdfPages(blobFile))
                .Returns(pages);

            _textCleanupServiceMock
            .Setup(x => x.CleanupText("raw page text"))
            .Returns("cleaned text");

            _chunkingServiceMock
                .Setup(x => x.ChunkText(fileName, 1, "cleaned text", 500))
                .Returns(chunkList);

            _embeddingServiceMock
                .Setup(x => x.GenerateEmbeddings("cleaned chunk content"))
                .ReturnsAsync(new float[] { 0.1f, 0.2f });

            _searchIndexServiceMock
                .Setup(x => x.IndexChunksAsync(It.IsAny<IEnumerable<Chunk>>()))
                .ReturnsAsync(Mock.Of<Azure.Search.Documents.Models.IndexDocumentsResult>());

            var result = await _documentProcessingService.ProcessDocument(Guid.NewGuid(), blobFile);

            var chunks = result.Chunks;
            Assert.Single(result.Chunks);
            Assert.Equal(fileName, chunks[0].FileName);
            Assert.Equal(1, chunks[0].PageNumber);
            Assert.Equal("cleaned chunk content", chunks[0].Content);
            Assert.Equal(1, chunks[0].ChunkIndex);
            Assert.NotEmpty(chunks[0].FileId);
            Assert.Equal(2, chunks[0].ContentVector.Length);

            _fileStorageServiceMock.Verify(x => x.DownloadDocument(_folderSettings.Value.DocumentsFolder, fileName), Times.Once);
            _pdfExtractionServiceMock.Verify(x => x.ExtractPdfPages(blobFile), Times.Once);
            _textCleanupServiceMock.Verify(x => x.CleanupText("raw page text"), Times.Once);
            _chunkingServiceMock.Verify(x => x.ChunkText(fileName, 1, "cleaned text", 500), Times.Once);
            _embeddingServiceMock.Verify(x => x.GenerateEmbeddings("cleaned chunk content"), Times.Once);
            _searchIndexServiceMock.Verify(x => x.IndexChunksAsync(It.IsAny<IEnumerable<Chunk>>()), Times.Once);
        }
    }
}
