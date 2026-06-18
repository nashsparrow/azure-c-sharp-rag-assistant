using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Contracts.Settings
{
    public class AzureApplicationInsightsSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

    }
}