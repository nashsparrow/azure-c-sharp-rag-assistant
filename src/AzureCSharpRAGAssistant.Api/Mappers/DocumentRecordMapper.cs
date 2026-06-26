using AzureCSharpRAGAssistant.Api.Contracts.Results;
using AzureCSharpRAGAssistant.Api.Models;

namespace AzureCSharpRAGAssistant.Api.Mappers
{
    public static class DocumentRecordMapper
    {
        public static DocumentResponse ToResponse(this DocumentRecord document) => new()
        {
            Id = document.Id,
            FileName = document.FileName,
            CreatedUtc = document.CreatedUtc,
            Indexed = document.Indexed
        };
    }
}