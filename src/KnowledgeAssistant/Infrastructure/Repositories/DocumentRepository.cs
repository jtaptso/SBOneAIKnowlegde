using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid documentId)
        {
            var documentToDelete = await _context.Documents.FindAsync(documentId);
            if (documentToDelete != null) { }
            {
                _context.Documents.Remove(documentToDelete!);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DocumentChunk>> GetChunksByDocumentIdAsync(Guid documentId)
        {
            return await _context.DocumentChunks
                .AsNoTracking()
                .Where(d => d.DocumentId == documentId)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
        {
            return await _context.Documents.FindAsync(documentId);
        }

        public async Task UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }
    }
}
