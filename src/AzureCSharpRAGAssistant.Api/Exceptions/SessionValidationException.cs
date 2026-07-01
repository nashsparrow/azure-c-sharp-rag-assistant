namespace AzureCSharpRAGAssistant.Api.Exceptions
{
    public class SessionValidationException : Exception
    {
        public SessionValidationException(string message) :
        base($"{message} You can retry after 24 hours from your last activity.")
        {
        }
    }
}