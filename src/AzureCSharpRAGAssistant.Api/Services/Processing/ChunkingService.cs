using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.Extensions.Options;
using PragmaticSegmenterNet;
using System.Text.RegularExpressions;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class ChunkingService : IChunkingService
    {
        private readonly ChunkSettings _chunkSettings;

        public ChunkingService(IOptions<ChunkSettings> chunkSettings)
        {
            _chunkSettings = chunkSettings.Value;
        }

        public IReadOnlyList<string> GetSentences(string text)
        {
            var sentences = Segmenter.Segment(
                text,
                Language.English,
                cleanText: true,
                documentType: DocumentType.Pdf);

            return sentences;
        }

        public List<Chunk>? ChunkText(string fileName, int pageNumber, string text)
        {
            //This method will overlap one sentence between each chunk of the page
            //This logic is implemented that the chunk text size is one sentence over than the chunk limit.
            
            var chunks = new List<Chunk>();
            var sentences = GetSentences(text);
            var maxChunkLength = _chunkSettings.ChunkSize;
            var chunkLength = 0;
            var chunkText = string.Empty;

            foreach (var sentence in sentences)
            {
                chunkText += (chunkText == string.Empty) ? sentence : " " + sentence;
                chunkLength += sentence.Length;
                
                if (chunkLength > maxChunkLength)
                {
                    chunks.Add(new Chunk
                    {
                        FileName = fileName,
                        PageNumber = pageNumber,
                        Content = chunkText
                    });

                    chunkLength = 0;
                    chunkText = sentence;
                }
            }

            chunks.Add(new Chunk
            {
                FileName = fileName,
                PageNumber = pageNumber,
                Content = chunkText
            });

            return chunks;
        }
    }
}
