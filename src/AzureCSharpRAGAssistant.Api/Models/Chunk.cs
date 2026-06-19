namespace AzureCSharpRAGAssistant.Api.Models
{
    public class Chunk
    {
        public string Id {get; set;} = Guid.NewGuid().ToString();
        public string FileId {get; set;} = string.Empty;
        public string FileName {get; set;} = string.Empty;
        public int ChunkIndex {get; set;}
        public int PageNumber {get; set;}
        public string Content {get; set;} = string.Empty;
        public int CharCount => Content.Length;
        public float[] ContentVector {get; set;} = [];
        public double? Score {get;set;}
    }
}