using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.Processing
{
    public class ChunkingService : IChunkingService
    {
        public List<Chunk> ChunkText(string text)
        {
            var chunks = new List<Chunk>();

            if (string.IsNullOrWhiteSpace(text))
            {
                return chunks;
            }

            var words = text.Split(
                new[] { ' ', '\r', '\n', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            const int chunkSize = 500;
            string previousLastSentence = string.Empty;

            for (int i = 0; i < words.Length; i += chunkSize)
            {
                var currentWords = words
                    .Skip(i)
                    .Take(chunkSize);

                var chunkText = string.Join(" ", currentWords);

                if (!string.IsNullOrWhiteSpace(previousLastSentence))
                {
                    chunkText = previousLastSentence + " " + chunkText;
                }

                chunks.Add(new Chunk
                {
                    Text = chunkText
                });

                previousLastSentence = GetLastSentence(chunkText);
            }

            return chunks;
        }

        private static string GetLastSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            var sentences = text.Split(
                new[] { '.', '!', '?' },
                StringSplitOptions.RemoveEmptyEntries);

            if (sentences.Length == 0)
            {
                return string.Empty;
            }

            return sentences[^1].Trim() + ".";
        }
    }
}