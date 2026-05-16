using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using OpenAI.Embeddings;

namespace Infrastructure.AI
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(IConfiguration config, EmbeddingClient embeddingClient)
        {
            _embeddingClient = embeddingClient;
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(text);

            return response.Value.ToFloats().ToArray();
        }
    }
}
