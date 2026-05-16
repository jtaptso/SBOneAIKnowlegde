using Application.Interfaces;
using Domain.DTO;
using Qdrant.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Qdrant_VectorDB
{
    public class QdrantVectorSearchService : IVectorSearchService
    {
        private readonly QdrantClient _qdrantClient;
        private const string CollectionName = "documents";
        private readonly IEmbeddingService _embeddingService;

        public QdrantVectorSearchService(QdrantClient qdrantClient, IEmbeddingService embeddingService)
        {
            _qdrantClient = qdrantClient;
            _embeddingService = embeddingService;
        }

        public async Task<List<VectorSearchResult>> SearchAsync(string query, int topK = 5)
        {
            var vector = await _embeddingService.GenerateEmbeddingAsync(query);

            var result = await _qdrantClient.SearchAsync(
                CollectionName,
                vector,
                limit: (uint)topK
                );

            return result.Select(x => new VectorSearchResult
            {
                Content = x.Payload["content"].ToString(),
                DocumentId = x.Payload["documentId"].ToString(),
                ChunkIndex = Convert.ToInt32(x.Payload["chunkIndex"]),
                Score = x.Score
            }).ToList();

            throw new NotImplementedException();
        }
    }
}
