using System.Text;
using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Services.ContextBuilder
{
    public class ContextBuilderService : IContextBuilderService
    {
        public ContextBuildResult BuildContext(List<Chunk> chunks)
        {
            var contextText = new StringBuilder();
            var sourcesList = new List<ReferenceSource>();

            foreach (var chunk in chunks)
            {
                contextText.Append(chunk.Content);
                sourcesList.Add(new ReferenceSource{ FileName = chunk.FileName, PageNumber = chunk.PageNumber, Score = chunk.Score});
            }

            return new ContextBuildResult
            {
                Context = contextText.ToString(),
                Sources = sourcesList
            };
        }
    }
}