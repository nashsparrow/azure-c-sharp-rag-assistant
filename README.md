# Azure C# RAG

Cloud-native Retrieval-Augmented Generation (RAG) system using ASP.NET Core, Azure OpenAI, Azure AI Search, and Azure Blob Storage for document ingestion, indexing, and retrieval workflows.

## Current implementation

The API currently supports:

- Uploading documents through `DocumentsController`
- Optional indexing during upload
- Running indexing manually through `IndexingController`
- Downloading documents from Azure Blob Storage for processing
- Extracting PDF text
- Cleaning and chunking text
- Generating embeddings
- Writing chunks into Azure AI Search
- Creating the Azure AI Search index at application startup if it does not already exist

## Project structure

```text
src/
  AzureCSharpRAGAssistant.Api/
    Controllers/
      DocumentsController.cs
      IndexingController.cs
    Services/
      Embedding/
      Extraction/
      Indexing/
      Processing/
      Storage/
    Models/
    Contracts/
      Requests/
      Results/
      Settings/

tests/
  AzureCSharpRAGAssistant.Api.Tests/
    Controllers/
      DocumentsControllerTests.cs
      IndexingControllerTests.cs
```

## Implemented API flow

### Document upload

`POST /api/documents/upload`

Current behavior:

1. Receives a file and indexing flag from the request form.
2. Uploads the file through `IFileStorageService`.
3. If indexing is enabled, triggers `IDocumentProcessingService.ProcessDocument(...)`.
4. Returns `200 OK` with the upload result.

### Manual indexing

`POST /api/indexing/run`

Current behavior:

1. Calls `IDocumentProcessingService.ProcessAllDocuments()`.
2. Returns `200 OK` with the produced chunk list.

## Service responsibilities

- `BlobStorageService`: uploads and downloads files from Azure Blob Storage
- `PdfExtrationService`: extracts pages and text from PDF documents
- `TextCleanupService`: normalizes extracted text before chunking
- `ChunkingService`: splits text into chunk objects
- `EmbeddingService`: generates vector embeddings for chunks
- `SearchIndexService`: writes chunks into Azure AI Search
- `SearchIndexManagementService`: ensures the Azure AI Search index exists
- `DocumentProcessingService`: orchestrates the end-to-end document processing pipeline

## Testing status

A dedicated test project exists at `tests/AzureCSharpRAGAssistant.Api.Tests`.

Current testing setup:

- `xUnit` as the test framework
- `Moq` for mocking dependencies
- controller unit tests started for:
  - `DocumentsController`
  - `IndexingController`

Current unit testing approach:

- controllers are tested by mocking service dependencies and asserting:
  - returned action results
  - whether dependent services were called
  - whether optional indexing behavior was triggered correctly

Planned next test areas:

- `DocumentProcessingService`
- `ChunkingService`
- `TextCleanupService`
- model behavior such as `Chunk.CharCount`
- configuration binding and settings validation

## Notes

- The application is built on `.NET 8`.
- Dependency injection is already in place for controllers and services, which makes unit testing straightforward.
- External integrations such as Azure OpenAI, Azure AI Search, and Azure Blob Storage should be mocked in unit tests and covered end-to-end with separate integration tests later.
