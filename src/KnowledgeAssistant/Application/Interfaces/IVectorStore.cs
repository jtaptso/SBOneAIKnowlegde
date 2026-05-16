using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IVectorStore
    {
        Task UpsertAsync(string documentId, int chunkIndex, string content, float[] vector);
        Task<List<string>> SearchAsync(float[] vector, int topK);
        Task InitializeAsync();
    }
}
