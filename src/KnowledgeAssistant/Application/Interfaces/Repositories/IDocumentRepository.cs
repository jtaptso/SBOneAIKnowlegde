using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document?> GetDocumentByIdAsync(Guid documentId);
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(Guid documentId);

        Task<IEnumerable<DocumentChunk>> GetChunksByDocumentIdAsync(Guid documentId);
    }
}
