using System.ClientModel;
using System.Text;
using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Models;
using AzureCSharpRAGAssistant.Api.Performance.Data.TestMetrics;
using AzureCSharpRAGAssistant.Api.Performance.Helpers;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using OpenAI.Realtime;
using UglyToad.PdfPig.Outline;

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
        private readonly EvaluationDataset _evaluationDataset;
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
            _evaluationDataset = LoadEvaluationDataset();

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

        private static EvaluationDataset LoadEvaluationDataset()
        {
            var candidatePaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "Performance", "Data", "TestMetrics", "evaluation.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "src", "AzureCSharpRAGAssistant.Api", "Performance", "Data", "TestMetrics", "evaluation.json")
            };

            var datasetPath = candidatePaths.FirstOrDefault(File.Exists)
                ?? throw new FileNotFoundException("Evaluation dataset file was not found.");

            var json = File.ReadAllText(datasetPath);
            var dataset = JsonSerializer.Deserialize<EvaluationDataset>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return dataset ?? throw new InvalidOperationException("Evaluation dataset file could not be deserialized.");
        }

        public async Task<IEnumerable<PdfPage>> ReadEvaluationFiles()
        {
            var fileList = new List<string>
            {
                "paper_1_orbital_agriculture.pdf",
                "paper_2_ocean_networks.pdf",
                "paper_3_materials_discovery.pdf"
            };

            var assembly = typeof(EvaluationPipelineService).Assembly;
            var pages = new List<PdfPage>();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var fileName in fileList)
            {
                var resourceName = resourceNames.FirstOrDefault(name =>
                    name.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    throw new InvalidOperationException($"Embedded resource for '{fileName}' not found.");
                }

                using var stream = assembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                var blobFile = new BlobFileResult
                {
                    FileName = fileName,
                    Content = memoryStream.ToArray()
                };

                var extractedPages = _pdfExtractionService.ExtractPdfPages(blobFile);
                pages.AddRange(extractedPages);
            }

            return pages;
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
            await SaveCreatedChunksAsync(chunkList, embeddingModel!, chunkSize);

            if (embeddingModel != null)
            {
                await SetEmbeddingsClient(embeddingModel);
            }

            var fileName = chunkList[0].FileName;
            var fileId = Guid.NewGuid().ToString();
            var pageNumber = chunkList[0].PageNumber;
            var chunkIndex = 0;

            foreach (var item in chunkList)
            {
                if (fileName == item.FileName)
                {
                    if (pageNumber == item.PageNumber)
                    {
                        item.ChunkIndex = chunkIndex++;
                    }
                    else
                    {
                        pageNumber = item.PageNumber;
                        chunkIndex = 0;
                        item.ChunkIndex = chunkIndex++;
                    }
                    item.FileId = fileId;
                }
                else
                {
                    fileName = item.FileName;
                    pageNumber = item.PageNumber;
                    chunkIndex = 0;
                    item.ChunkIndex = chunkIndex++;
                    fileId = Guid.NewGuid().ToString();
                    item.FileId = fileId;
                }
                item.ContentVector = await GenerateEmbeddings(item.Content);
            }

            // index pages
            var res = await IndexPages(chunks.ToList());

            return chunks.ToList();
        }

        public async Task<List<Chunk>> SearchIndexes(string question, int topK)
        {
            var result = await _searchIndexService.SearchChunksAsync(question, topK, true);
            return result;
        }

        private static async Task SaveCreatedChunksAsync(IEnumerable<Chunk> createdChunks, string embeddingDeployment, int chunkSize)
        {
            var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", "result");
            Directory.CreateDirectory(resultsDirectory);

            var safeEmbeddingName = string.Concat(
                embeddingDeployment.Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));

            var fileName = $"{safeEmbeddingName}_chunksize_{chunkSize}.json ";
            var filePath = Path.Combine(resultsDirectory, fileName);

            var json = JsonSerializer.Serialize(
                createdChunks,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(filePath, json);
        }

        private static async Task SaveEvaluationSummaryAsync(string summaryText, string embeddingDeployment, int chunkSize)
        {
            var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output", "result");
            Directory.CreateDirectory(resultsDirectory);

            var safeEmbeddingName = string.Concat(
                embeddingDeployment.Select(character => Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));

            var fileName = $"{safeEmbeddingName}_chunksize_{chunkSize}_summary.txt";
            var filePath = Path.Combine(resultsDirectory, fileName);

            await File.WriteAllTextAsync(filePath, summaryText);
        }

        public float CalculateMRRValue(int rankForMRR)
        {
            if (rankForMRR < 1)
            {
                return 0;
            }
            else
            {
                return 1 / (float)rankForMRR;
            }
        }

        public float CalculatePrecisionValue(int relevantChunks, int topK)
        {
            if (relevantChunks < 1)
            {
                return 0;
            }
            else
            {
                return relevantChunks / (float)topK;
            }
        }

        public async Task RunRecallPrecisionMRRHitBoundaryFalseEvaluation(StringBuilder stringBuilder, int topK)
        {
            var totalTests = 0;
            var matchedAll = 0;
            var partialyMatched = 0;
            var matchedNone = 0;
            var mRR_total = 0f;
            var precision_total = 0f;
            var recall_total = 0f;

            var boundaryTotalTests = 0;
            var boundayPassed = 0;

            var noAnswerTotalTests = 0;
            var noAnswerPassed = 0;


            //type Main tests
            foreach (var test in _evaluationDataset.Tests)
            {
                var resultChunks = await SearchIndexes(test.Question, topK);

                if (test.Categories.Contains(EvaluationCategories.single_hop.ToString()) || test.Categories.Contains(EvaluationCategories.multi_hop.ToString()))
                {
                    totalTests++;
                    var compareResult = CompareChunks(test.ExpectedChunkContains, resultChunks);
                    mRR_total += CalculateMRRValue(compareResult.rankForMRR);
                    precision_total += CalculatePrecisionValue(compareResult.relevantChunks, topK);
                    recall_total += compareResult.hitRatio;

                    switch (compareResult.result)
                    {
                        case EvaluationResults.matched_non:
                            matchedNone++;
                            break;
                        case EvaluationResults.all_matched:
                            matchedAll++;
                            break;
                        case EvaluationResults.partially_matched:
                            partialyMatched++;
                            break;

                        default:
                            break;
                    }
                }

                if (test.Categories.Contains(EvaluationCategories.boundary.ToString()))
                {
                    boundaryTotalTests++;
                    var compareResult = CompareChunksForBoundaryEvaluation(test.BoundaryTermsMustBeTogether, resultChunks);
                    if (compareResult)
                    {
                        boundayPassed++;
                    }
                }

                if (test.Categories.Contains(EvaluationCategories.no_answer.ToString()))
                {
                    noAnswerTotalTests++;
                    var compareResult = CompareChunksForNoAnswerFalsePositiveEvaluation(resultChunks);
                    if (compareResult == true)
                    {
                        noAnswerPassed++;
                    }
                }
            }

            var hitAtK = ((float)partialyMatched + matchedAll) / totalTests;
            var recallatValue = recall_total / totalTests;
            var mRRMean = mRR_total / totalTests;
            var precisionMean = precision_total / totalTests;
            var boundaryMean = (float)boundayPassed / boundaryTotalTests;
            var noAnswerMean = (float)noAnswerPassed / noAnswerTotalTests;

            stringBuilder.AppendLine($"Recall@{topK} : ratio: {recallatValue}  percentage: {Math.Round(recallatValue * 100, 2)}%");
            stringBuilder.AppendLine($"MRR@{topK} : ratio: {mRRMean}  percentage: {Math.Round(mRRMean * 100, 2)}%");
            stringBuilder.AppendLine($"Pricision@{topK} : ratio: {precisionMean} percentage: {Math.Round(precisionMean * 100, 2)}%");
            stringBuilder.AppendLine($"Hit@{topK} : ratio: {hitAtK} percentage: {Math.Round(hitAtK * 100, 2)}%");
            stringBuilder.AppendLine($"BoundaryRecall@{topK} : ratio: {boundaryMean} percentage: {Math.Round(boundaryMean * 100, 2)}%");
            stringBuilder.AppendLine($"NoAnswerFalsePositive@{topK} : ratio: {noAnswerMean} percentage: {Math.Round(noAnswerMean * 100, 2)}%");

            stringBuilder.AppendLine($"Recall@{topK},Pricision@{topK}, MRR@{topK},Hit@{topK},BoundaryRecall@{topK},NoAnswerFalsePositive@{topK}");
            stringBuilder.AppendLine($"{Math.Round(recallatValue * 100, 2)},{Math.Round(precisionMean * 100, 2)},{Math.Round(mRRMean * 100, 2)},{Math.Round(hitAtK * 100, 2)},{Math.Round(boundaryMean * 100, 2)},{Math.Round(noAnswerMean * 100, 2)}");

            Console.WriteLine("Recall@{0} : ratio: {1}", topK, recallatValue);
        }

        // this method will true - passed, false - fails
        private bool? CompareChunksForNoAnswerFalsePositiveEvaluation(List<Chunk> chunks)
        {
            var threshold = 0.60f;

            if (chunks.Count() == 0)
            {
                return true;
            }

            Console.WriteLine("Chunk Score {0}", chunks[0].Score);
            var isAllScoresBelowThreshold = chunks?.All(x =>
                        x.Score <= threshold);

            return isAllScoresBelowThreshold;
        }

        // this method will true - passed, false - fails
        private bool CompareChunksForBoundaryEvaluation(List<string>? expectedChunkContains, List<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                var isMatch = expectedChunkContains?.All(word =>
                        chunk.Content.Contains(word, StringComparison.OrdinalIgnoreCase));

                if (isMatch == true)
                {
                    return true;
                }
            }

            return false;
        }

        private (EvaluationResults result, float hitRatio, int rankForMRR, int relevantChunks) CompareChunks(List<List<string>> expectedChunkContains, List<Chunk> chunks)
        {
            var totalExpected = expectedChunkContains.Count();
            var foundWords = new List<List<string>>();
            var foundWordsCount = 0;
            var chunkRank = 0;
            var foundRank = -1;
            var relevantChunks = 0;
            foreach (var chunk in chunks)
            {
                chunkRank++;
                foreach (var expectedWords in expectedChunkContains)
                {
                    var isMatch = expectedWords.All(word =>
                        chunk.Content.Contains(word, StringComparison.OrdinalIgnoreCase));

                    if (isMatch)
                    {
                        if (foundWords.Contains(expectedWords))
                        {
                            relevantChunks++;
                            break;
                        }
                        else
                        {
                            foundWords.Add(expectedWords);
                            relevantChunks++;
                            foundRank = chunkRank;
                            foundWordsCount++;
                            break;
                        }
                    }
                }
            }

            if (foundWordsCount == totalExpected)
            {
                return (EvaluationResults.all_matched, 1, foundRank, relevantChunks);
            }
            else if (foundWordsCount == 0)
            {
                return (EvaluationResults.matched_non, 0, foundRank, relevantChunks);
            }
            else
            {
                return (EvaluationResults.partially_matched, (float)foundWordsCount / totalExpected, foundRank, relevantChunks);
            }
        }

        public async Task RunAllEvaluations()
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
                    var summaryBuilder = new StringBuilder();
                    summaryBuilder.AppendLine($"==============================================");
                    summaryBuilder.AppendLine($"Evaluation run started: {DateTime.UtcNow:O}");
                    summaryBuilder.AppendLine($"Embedding deployment: {embeddingDeployment}");
                    summaryBuilder.AppendLine($"Chunk size: {chunkSize}");

                    //refresh the test index as a new index for each chunk size
                    var result = await _builder.CreatePerformanceEnvironment();
                    summaryBuilder.AppendLine($"Performance environment created: {result}");

                    var createdChunks = await RunTestPipeLineFromChunking(pages, chunkSize, embeddingDeployment);
                    summaryBuilder.AppendLine($"Chunks created: {createdChunks.Count}");
                    summaryBuilder.AppendLine("Chunk output saved to results folder.");

                    //For each test in the tests
                    foreach (var test in config?.Tests!)
                    {
                        summaryBuilder.AppendLine($"Processing test: {test.Name}");

                        if (test.Name == "@Tests")
                        {
                            var scenarios = test.Scenarios?
                                .Select(scenario => new { scenario.Name, scenario.TopK })
                                .ToArray() ?? [];

                            //For each scenario in each test category
                            foreach (var scenario in scenarios)
                            {
                                summaryBuilder.AppendLine($"Running Recall Precision MRR Hit Bounday False scenario: {scenario.Name} with topK={scenario.TopK}");
                                await RunRecallPrecisionMRRHitBoundaryFalseEvaluation(summaryBuilder, scenario.TopK);
                                summaryBuilder.AppendLine($"Completed Recall Precision MRR Hit Bounday False scenario: {scenario.Name}");
                            }
                        }
                    }

                    summaryBuilder.AppendLine($"Evaluation run completed: {DateTime.UtcNow:O}");
                    await SaveEvaluationSummaryAsync(summaryBuilder.ToString(), embeddingDeployment, chunkSize);
                }
            }
        }
    }
}
