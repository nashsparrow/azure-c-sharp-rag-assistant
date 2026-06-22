using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Data;
using AzureCSharpRAGAssistant.Api.Filters;
using AzureCSharpRAGAssistant.Api.Middleware;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Factory;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Plugins;
using AzureCSharpRAGAssistant.Api.SemanticKernel.Services;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using AzureCSharpRAGAssistant.Api.Services.ChatHistories;
using AzureCSharpRAGAssistant.Api.Services.ContextBuilder;
using AzureCSharpRAGAssistant.Api.Services.Documents;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAppDBContext>(sp => sp.GetRequiredService<AppDBContext>());

// Register Settings
builder.Services
    .AddOptions<AzureStorageSettings>()
    .Bind(builder.Configuration.GetSection("AzureStorage"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<AzureSearchSettings>()
    .Bind(builder.Configuration.GetSection("AzureSearch"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<AzureOpenAISettings>()
    .Bind(builder.Configuration.GetSection("AzureOpenAI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<FolderSettings>()
    .Bind(builder.Configuration.GetSection("Folders"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<AzureApplicationInsightsSettings>()
    .Bind(builder.Configuration.GetSection("ApplicationInsights"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<ChunkSettings>()
    .Bind(builder.Configuration.GetSection("ChunkSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
builder.Services.AddScoped<IFileStorageService, BlobStorageService>();
builder.Services.AddScoped<ITextCleanupService, TextCleanupService>();
builder.Services.AddScoped<IPdfExtractionService, PdfExtractionService>();
builder.Services.AddScoped<IDocumentProcessingService, DocumentProcessingService>();
builder.Services.AddScoped<IChunkingService, ChunkingService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<ISearchIndexManagementService, SearchIndexManagementService>();
builder.Services.AddScoped<ISearchIndexService, SearchIndexService>();
builder.Services.AddScoped<IContextBuilderService, ContextBuilderService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChatHistoriesService, ChatHistoriesService>();
builder.Services.AddScoped<IDocumentRecordsService, DocumentRecordsService>();
builder.Services.AddScoped<IDocumentsUploadService, DocumentsUploadService>();
builder.Services.AddScoped<IKernelFactory, KernelFactory>();
builder.Services.AddScoped<DocumentSearchPlugin>();
builder.Services.AddScoped<ISKAnswerService, SKAnswerService>();

builder.Services.AddScoped<ValidateFileUploadFilter>();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    const int maxMigrationAttempts = 10;

    for (var attempt = 1; attempt <= maxMigrationAttempts; attempt++)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            await dbContext.Database.MigrateAsync();
            break;
        }
        catch (Exception ex) when (attempt < maxMigrationAttempts)
        {
            logger.LogWarning(ex,
                "Database migration attempt {Attempt} of {MaxAttempts} failed. Retrying in 5 seconds.",
                attempt,
                maxMigrationAttempts);

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }

    // This will search the index in Azure Search, if it does not exist it will create new index
    var indexManager = scope.ServiceProvider.GetRequiredService<ISearchIndexManagementService>();
    await indexManager.EnsureIndexExistsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
