using Application.Interfaces;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Qdrant_VectorDB
{
    public class QdrantVectorStore : IVectorStore
    {
        private readonly QdrantClient _client;
        private const string CollectionName = "sapb1_docs";

        public QdrantVectorStore(QdrantClient client)
        {
            _client = client;
        }

        public async Task<List<string>> SearchAsync(float[] vector, int topK)
        {
            var results = await _client.SearchAsync(CollectionName, vector, limit: (ulong)topK);
            return results.Select(r => r.Payload["content"].StringValue).ToList();   
        }

        public async Task UpsertAsync(string documentId, int chunkIndex, string content, float[] vector)
        {
            var point = new PointStruct
            {
                Id = (ulong)Guid.NewGuid().GetHashCode(),
                Vectors = vector,
                Payload =
                {
                    ["documentId"] = documentId,
                    ["chunkIndex"] = chunkIndex,
                    ["content"] = content
                }
            };
            await _client.UpsertAsync(CollectionName, new[] { point });
        }

        public async Task InitializeAsync()
        {
            var collections = await _client.ListCollectionsAsync();

            bool exists = collections.Contains(CollectionName);

            if (!exists) 
            { 
                await _client.CreateCollectionAsync(CollectionName, new VectorParams
                {
                    Size = 1536,
                    Distance = Distance.Cosine
                });
            }
        }
    }
}
