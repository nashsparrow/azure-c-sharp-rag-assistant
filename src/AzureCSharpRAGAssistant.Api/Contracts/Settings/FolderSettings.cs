using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Contracts.Settings
{
    public class FolderSettings
    {
        public string DocumentsFolder { get; set; } = string.Empty;
        public string OutputFolder { get; set; } = string.Empty;
    }
}