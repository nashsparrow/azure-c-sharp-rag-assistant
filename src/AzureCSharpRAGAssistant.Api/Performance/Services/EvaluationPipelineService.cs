using System.ClientModel;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Performance.Helpers;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace AzureCSharpRAGAssistant.Api.Performance.Services
{
    public class EvaluationPipelineService : IEvaluationPipelineService
    {
        private sealed class EvaluationConfig
        {
            public List<EvaluationTests>? Tests { get; set; }
            public List<string>? EmbeddingDeployments { get; set; }
            public List<string>? ChatDeployments { get; set; }
            public List<int>? ChunkSizes { get; set; }
        }

        private sealed class EvaluationTests
        {
            public string? Name { get; set; }
            public List<RecallScenario>? Scenarios { get; set; }
        }

        private sealed class RecallScenario
        {
            public string? Name { get; set; }
            public int TopK { get; set; }
        }

        private readonly ISearchIndexManagementService _searchIndexManagementService;
        private readonly AzureSearchSettings _azureSearchSettings;
        private readonly PerformanceEnvironmentBuilder _builder;
        private readonly IPdfExtractionService _pdfExtractionService;
        private readonly IChunkingService _chunkingService;
        private readonly SearchClient _searchClient;
        private readonly AzureOpenAISettings _azureOpenAISettings;
        private EmbeddingClient? IndexEmbeddingClient { get; set; } = null; //This is initiated as a property, so The evaluation can be run with different embedding models
        private AzureOpenAIClient AzureClient { get; }
        private ISearchIndexService _searchIndexService;

        public EvaluationPipelineService(ISearchIndexManagementService searchIndexManagementService, IOptions<AzureSearchSettings> azureSearchSettings,
            IPdfExtractionService pdfExtractionService, IChunkingService chunkingService, IOptions<AzureOpenAISettings> azureOpenAISettings,
            ISearchIndexService searchIndexService)
        {
            _searchIndexManagementService = searchIndexManagementService;
            _azureSearchSettings = azureSearchSettings.Value;
            _builder = new PerformanceEnvironmentBuilder(_searchIndexManagementService);
            _pdfExtractionService = pdfExtractionService;
            _chunkingService = chunkingService;
            _searchIndexService = searchIndexService;

            _searchClient = new SearchClient(
                new Uri(_azureSearchSettings.Endpoint),
                _azureSearchSettings.IntegrationTestIndexName,
                new Azure.AzureKeyCredential(_azureSearchSettings.ApiKey)
                );

            _azureOpenAISettings = azureOpenAISettings.Value;

            AzureClient = new AzureOpenAIClient(
                new Uri(_azureOpenAISettings.Endpoint),
                new ApiKeyCredential(_azureOpenAISettings.ApiKey));

            IndexEmbeddingClient = AzureClient.GetEmbeddingClient(_azureOpenAISettings.EmbeddingDeployment);
        }

        public async Task<IEnumerable<PdfPage>> ReadEvaluationFiles()
        {
            var fileList = new List<string> { "" };
            var assembly = typeof(EvaluationPipelineService).Assembly;
            var resourceName = "AzureCSharpRAGAssistant.Api.IntegrationTests.Services.Extraction.TestFiles.test_research_paper.pdf";

            using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var blobFile = new BlobFileResult
            {
                FileName = "sample_paper_loop_unrolling.pdf",
                Content = memoryStream.ToArray()
            };

            return _pdfExtractionService.ExtractPdfPages(blobFile);
        }

        public async Task<IEnumerable<Chunk>> ChunkPages(List<PdfPage> pages, int chunkSize)
        {
            var chunkList = new List<Chunk>();
            foreach (var page in pages)
            {
                var chunk = _chunkingService.ChunkText(page.FileName, page.PageNumber, page.Text, chunkSize);
                if (chunk != null)
                {
                    chunkList.AddRange(chunk);
                }
            }

            return chunkList;
        }

        public async Task<IndexDocumentsResult> IndexPages(List<Chunk> chunks)
        {
            var result = await _searchClient.UploadDocumentsAsync(chunks);
            return result;
        }

        public async Task SetEmbeddingsClient(string embeddingDeployment)
        {
            IndexEmbeddingClient = AzureClient.GetEmbeddingClient(embeddingDeployment);
        }

        public async Task<float[]> GenerateEmbeddings(string text)
        {
            if (IndexEmbeddingClient == null)
            {
                throw new Exception("Embedding Client Should be Initialized before Generating Embedding");
            }
            OpenAIEmbedding embedding = await IndexEmbeddingClient!.GenerateEmbeddingAsync(text);
            return embedding.ToFloats().ToArray();
        }

        public async Task<List<Chunk>> RunTestPipeLineFromChunking(IEnumerable<PdfPage> pages, int chunkSize, string? embeddingModel = null)
        {
            var chunks = await ChunkPages(pages.ToList(), chunkSize);

            var chunkList = chunks.ToList();

            if (embeddingModel != null)
            {
                await SetEmbeddingsClient(embeddingModel);
            }

            foreach (var item in chunkList)
            {
                item.ContentVector = await GenerateEmbeddings(item.Content);
            }

            // index pages
            var res = await IndexPages(chunks.ToList());

            return chunks.ToList();
        }

        public async Task<List<Chunk>> SearchIndexes(string question, int topK)
        {
            var result = await _searchIndexService.SearchChunksAsync(question, topK);
            return result;
        }

        public async Task RunRecallEvaluation(int topK)
        {

        }

        public async Task RunAllEvaluations(bool runRecallEvaluations = true)
        {
            var candidatePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Performance", "Data", "EvaluationConfig", "evaluation-config.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "src", "AzureCSharpRAGAssistant.Api", "Performance", "Data", "EvaluationConfig", "evaluation-config.json")
            };

            var configPath = candidatePaths.FirstOrDefault(File.Exists)
                ?? throw new FileNotFoundException("Evaluation config file was not found.");

            var configJson = await File.ReadAllTextAsync(configPath);
            var config = JsonSerializer.Deserialize<EvaluationConfig>(
                configJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            //preload the pages //testing does not effect the loaded pages
            var pages = await ReadEvaluationFiles();

            // For every embedding deployment model in the config file
            foreach (var embeddingDeployment in config?.EmbeddingDeployments!)
            {
                //For each chunksize in the config file
                foreach (var chunkSize in config?.ChunkSizes!)
                {
                    //refresh the test index as a new index for each chunk size
                    var result = await _builder.CreatePerformanceEnvironment();
                    var createdChunks = await RunTestPipeLineFromChunking(pages, chunkSize, embeddingDeployment);

                    //For each test in the tests
                    foreach (var test in config?.Tests!)
                    {
                        if (test.Name == "Recall@ Tests" && runRecallEvaluations)
                        {
                            var scenarios = test.Scenarios?
                                .Select(scenario => new { scenario.Name, scenario.TopK })
                                .ToArray() ?? [];

                            //For each scenario in each test category
                            foreach (var scenario in scenarios)
                            {
                                await RunRecallEvaluation(scenario.TopK);
                            }
                        }
                    }
                }
            }
        }
    }
}
