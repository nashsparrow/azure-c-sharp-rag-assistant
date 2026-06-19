using AzureCSharpRAGAssistant.Api.Contracts;
using AzureCSharpRAGAssistant.Api.Contracts.Settings;
using AzureCSharpRAGAssistant.Api.Services;
using AzureCSharpRAGAssistant.Api.Services.Chat;
using AzureCSharpRAGAssistant.Api.Services.ContextBuilder;
using AzureCSharpRAGAssistant.Api.Services.Embedding;
using AzureCSharpRAGAssistant.Api.Services.Indexing;
using AzureCSharpRAGAssistant.Api.Services.Processing;
using AzureCSharpRAGAssistant.Api.Services.Storage;

var builder = WebApplication.CreateBuilder(args);


// Register Settings
builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection("AzureStorage"));

builder.Services.Configure<AzureSearchSettings>(
    builder.Configuration.GetSection("AzureSearch"));

builder.Services.Configure<AzureOpenAISettings>(
    builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.Configure<FolderSettings>(
    builder.Configuration.GetSection("Folders"));

builder.Services.Configure<AzureApplicationInsightsSettings>(
    builder.Configuration.GetSection("ApplicationInsights"));

builder.Services.Configure<ChunkSettings>(
    builder.Configuration.GetSection("ChunkSettings"));

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

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
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

app.UseAuthorization();

app.MapControllers();

app.Run();
