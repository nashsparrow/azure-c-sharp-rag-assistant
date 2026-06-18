using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Services.Processing;

namespace AzureCSharpRAGAssistant.Api.Tests.Services.Processing
{
    public class ChunkingServiceTests
    {
        private readonly ChunkingService _chunkingService;
        public ChunkingServiceTests()
        {
            _chunkingService = new ChunkingService();
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
    }
}