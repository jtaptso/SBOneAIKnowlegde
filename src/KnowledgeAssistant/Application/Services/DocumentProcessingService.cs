using Application.Interfaces;
using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DocumentProcessingService : IDocumentProcessingService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IPdfExtractionService _pdfService;
        private readonly ITextChunkingService _chunkingService;
        private readonly IVectorStore _vectorStore;
        private readonly IEmbeddingService _embeddingService;   

        public DocumentProcessingService(
            IDocumentRepository documentRepository, 
            IPdfExtractionService pdfService,
            ITextChunkingService chunkingService,
            IVectorStore vectorStore,
            IEmbeddingService embeddingService)
        {
            _documentRepository = documentRepository;
            _pdfService = pdfService;
            _chunkingService = chunkingService;
            _vectorStore = vectorStore;
            _embeddingService = embeddingService;
        }
        public async Task ProcessDocumentAsync(Guid documentId)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document == null) 
                throw new Exception("Document not found");
            
            //Extract text from Pdf file
            var text = await _pdfService.ExtractTextAsync(document.FileName);

            //chunk the extracted part
            var chunks = _chunkingService.ChunkText(text);

            for (int i = 0; i < chunks.Count; i++) 
            {
                var chunk = chunks[i];
                var vector = await _embeddingService.GenerateEmbeddingAsync(chunk);

                // Add the vector representation of the chunk in Qdrant
                await _vectorStore.UpsertAsync(document.Id.ToString(), i, chunk, vector);
            }
        }
    }
}
