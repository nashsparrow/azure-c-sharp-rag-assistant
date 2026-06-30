namespace AzureCSharpRAGAssistant.Api.Exceptions
{
    public class SessionValidationException : Exception
    {
        public SessionValidationException(string message) : base(message)
        {
        }
    }
}