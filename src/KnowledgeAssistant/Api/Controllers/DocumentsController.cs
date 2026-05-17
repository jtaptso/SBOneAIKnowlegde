using Application.Interfaces;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            IDocumentRepository documentRepository,
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment env,
            ILogger<DocumentsController> logger)
        {
            _documentRepository = documentRepository;
            _scopeFactory = scopeFactory;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Uploads a PDF document, saves it to disk, stores metadata in the database,
        /// and triggers the RAG ingestion pipeline (extract → chunk → embed → index).
        /// </summary>
        [HttpPost("upload")]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<IActionResult> Upload(
            [FromForm] IFormFile file,
            [FromForm] string? uploadedBy = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided." });

            // Require a .pdf extension (harder to spoof than ContentType alone)
            if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only PDF files (.pdf) are accepted." });

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "File content type must be application/pdf." });

            var uploadsDir = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsDir);

            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, uniqueFileName);

            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = uploadedBy ?? "system"
            };

            // Save metadata to DB first
            await _documentRepository.AddAsync(document);

            // Save the file to disk after DB succeeds (avoids orphaned files)
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trigger the RAG ingestion pipeline in the background
            // Capture scope factory before the background task to avoid accessing
            // HttpContext (which may be disposed) from the background thread.
            var docId = document.Id;
            var scopeFactory = _scopeFactory;
            var logger = _logger;

            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IDocumentProcessingService>();
                    await processor.ProcessDocumentAsync(docId);
                    logger.LogInformation("Document {Id} processed successfully", docId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to process document {Id}", docId);
                }
            });

            return Ok(new
            {
                documentId = document.Id,
                fileName = document.FileName,
                filePath = document.FilePath,
                uploadedAt = document.UploadedAt,
                message = "Document uploaded successfully. Processing started."
            });
        }
    }
}
