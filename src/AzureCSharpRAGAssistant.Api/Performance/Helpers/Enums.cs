namespace AzureCSharpRAGAssistant.Api.Performance.Helpers
{
    public enum EvaluationCategories
    {
        single_hop = 1,
        numeric = 2,
        entity = 3,
        multi_hop = 4,
        boundary = 5,
        no_answer = 6
    }

    public enum EvaluationResults
    {
        all_matched = 1,
        partially_matched = 2,
        matched_non = 3
    }
}