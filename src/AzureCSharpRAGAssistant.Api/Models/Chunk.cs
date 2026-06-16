using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureCSharpRAGAssistant.Api.Models
{
    public class Chunk
    {
        public Guid FileId {get; set;} = new Guid();
        public string FileName {get; set;} = string.Empty;
        public Guid ChunkId {get; set;} = new Guid();
        public string ChunkIndex {get; set;} = string.Empty;
        public int PageNumber {get; set;}
        public string Text {get; set;} = string.Empty;
        public int CharCount => Text.Length;
    }
}