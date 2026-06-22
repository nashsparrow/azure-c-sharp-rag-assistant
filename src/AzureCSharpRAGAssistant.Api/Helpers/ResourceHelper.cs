using System.Reflection;

namespace AzureCSharpRAGAssistant.Api.Helpers
{
    public static class ResourceHelper
    {
        public static async Task<string> ReadEmbeddedResourceAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            await using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' was not found.");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}