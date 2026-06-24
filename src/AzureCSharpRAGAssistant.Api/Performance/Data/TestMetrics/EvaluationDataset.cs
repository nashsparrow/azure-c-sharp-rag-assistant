namespace AzureCSharpRAGAssistant.Api.Performance.Data.TestMetrics
{
    public class EvaluationDataset
    {
        public string DatasetName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<EvaluationSourceFile> SourceFiles { get; set; } = [];
        public EvaluationRecommendedMetrics? RecommendedMetrics { get; set; }
        public EvaluationScoringNotes? ScoringNotes { get; set; }
        public List<EvaluationTestCase> Tests { get; set; } = [];
    }

    public class EvaluationSourceFile
    {
        public string PaperId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }

    public class EvaluationRecommendedMetrics
    {
        public List<int> KValues { get; set; } = [];
        public List<string> Metrics { get; set; } = [];
    }

    public class EvaluationScoringNotes
    {
        public string ExpectedChunkContains { get; set; } = string.Empty;
        public string Single_Hop { get; set; } = string.Empty;
        public string Multi_Hop { get; set; } = string.Empty;
        public string Precision { get; set; } = string.Empty;
        public string Mrr { get; set; } = string.Empty;
        public string Boundary { get; set; } = string.Empty;
        public string No_Answer { get; set; } = string.Empty;
    }

    public class EvaluationTestCase
    {
        public string Id { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = [];
        public string Difficulty { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public List<string> ExpectedFacts { get; set; } = [];
        public List<List<string>> ExpectedChunkContains { get; set; } = [];
        public EvaluationExpectedSource? ExpectedSource { get; set; }
        public List<string>? BoundaryTermsMustBeTogether { get; set; }
        public string? ExpectedAnswerBehavior { get; set; }
    }

    public class EvaluationExpectedSource
    {
        public string FileName { get; set; } = string.Empty;
    }
}
