using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Contracts
{
    public class AzureSearchSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string IndexName { get; set; } = string.Empty;
    }
}