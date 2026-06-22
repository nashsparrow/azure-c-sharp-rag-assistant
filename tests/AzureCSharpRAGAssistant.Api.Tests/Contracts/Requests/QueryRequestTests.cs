using System.ComponentModel.DataAnnotations;
using AzureCSharpRAGAssistant.Api.Contracts.Requests;

namespace AzureCSharpRAGAssistant.Api.Tests.Contracts.Requests
{
    public class QueryRequestTests
    {
        [Theory]
        [InlineData(" ")]
        [InlineData("     ")]
        [InlineData("")]
        public void Test_QueryRequestShouldFail_WhenQuestionIsEmpty(string question)
        {
            var request = new QueryRequest
            {
                Question = question
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(QueryRequest.Question)));
        }

        [Theory]
        [InlineData("what?")]
        [InlineData("yes?")]
        [InlineData("no?")]
        public void Test_QueryRequestShouldFail_WhenQuestionIsShort(string question)
        {
            var request = new QueryRequest
            {
                Question = question
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(QueryRequest.Question)));
        }

        [Theory]
        [InlineData("what is loop unrolling?")]
        [InlineData("what is DQN?")]
        [InlineData("Common RL Methods")]
        public void Test_QueryRequestShouldPass_WhenQuestionIsPassable(string question)
        {
            var request = new QueryRequest
            {
                Question = question
            };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(request, context, results, true);

            Assert.True(isValid);
        }
    }
}