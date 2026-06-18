using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Models;
using Microsoft.Extensions.Options;
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

        public List<Chunk>? ChunkText(string fileName, int pageNumber, string text)
        {
            var chunks = new List<Chunk>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            var sentences = SplitSentences(text);
            if (sentences.Count == 0)
            {
                return chunks;
            }

            var maxChunkLength = Math.Max(_chunkSettings.ChunkSize * 2, 1);
            var startIndex = 0;

            while (startIndex < sentences.Count)
            {
                var chunkSentences = new List<string>();
                var currentIndex = startIndex;

                while (currentIndex < sentences.Count)
                {
                    var candidateSentences = chunkSentences.Append(sentences[currentIndex]);
                    var candidateText = string.Join(" ", candidateSentences);

                    if (chunkSentences.Count > 0 && candidateText.Length > maxChunkLength)
                    {
                        break;
                    }

                    chunkSentences.Add(sentences[currentIndex]);
                    currentIndex++;
                }

                chunks.Add(new Chunk
                {
                    FileName = fileName,
                    PageNumber = pageNumber,
                    Content = string.Join(" ", chunkSentences)
                });

                if (currentIndex >= sentences.Count)
                {
                    break;
                }

                startIndex = chunkSentences.Count > 1
                    ? currentIndex - 1
                    : currentIndex;
            }

            return chunks;
        }

        private static List<string> SplitSentences(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return [];
            }

            var matches = Regex.Matches(text, @"[^.!?]+[.!?]|[^.!?]+$");
            if (matches.Count == 0)
            {
                return [];
            }

            return matches
                .Select(match => match.Value.Trim())
                .Where(sentence => !string.IsNullOrWhiteSpace(sentence))
                .ToList();
        }
    }
}
