using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Services.Indexing
{
    public interface ISearchIndexManagementService
    {
        Task EnsureIndexExistsAsync();
    }
}