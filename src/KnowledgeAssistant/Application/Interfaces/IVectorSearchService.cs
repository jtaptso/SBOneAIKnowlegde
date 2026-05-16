using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IVectorSearchService
    {
        Task<List<VectorSearchResult>> SearchAsync(string query, int topK = 5);
    }
}
