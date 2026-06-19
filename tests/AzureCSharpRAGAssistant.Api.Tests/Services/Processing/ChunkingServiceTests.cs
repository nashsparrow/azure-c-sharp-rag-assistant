using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.Extensions.Options;

namespace AzureCSharpRAGAssistant.Api.Tests.Services.Processing
{
    public class ChunkingServiceTests
    {
        private readonly ChunkingService _chunkingService;
        private IOptions<ChunkSettings> _chunkSettings;
        public ChunkingServiceTests()
        {
            _chunkSettings = Options.Create( new ChunkSettings { ChunkSize = 15 });
            _chunkingService = new ChunkingService(_chunkSettings);
        }

        [Fact]
        public void ChunkText_ReturnsSingleChunk_For_SingleSentence_UnderTheCharacterLimit()
        {
            var text = "Single Text.";
            var result = _chunkingService.ChunkText("testfile.pdf", 1, text);
            Assert.IsType<List<Chunk>>(result);
            Assert.Single(result);
            Assert.Equal("Single Text.", result[0].Content);
        }

        [Fact]
        public void ChunkText_ReturnsTwoChunks_For_Sentences_ExceedTheCharacterLimit_ButDoesNotExceedDoubleOfTheCharacterLimit()
        {
            var text = "Single Text. Second Text.";
            var result = _chunkingService.ChunkText("testfile.pdf", 1, text);
            Assert.IsType<List<Chunk>>(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Single Text. Second Text.", result[0].Content);
            Assert.Equal("Second Text.", result[1].Content);
        }

        [Fact]
        public void ChunkText_ReturnsChunks_WithOverlappingSentence()
        {
            var text = "Single Text. Second Text. Third Text.";
            var result = _chunkingService.ChunkText("testfile.pdf", 1, text);
            Assert.IsType<List<Chunk>>(result);
            Assert.Equal(2 , result.Count());
            Assert.Equal("Single Text. Second Text.", result[0].Content);
            Assert.Equal("Second Text. Third Text.", result[1].Content);
        }
    }
}