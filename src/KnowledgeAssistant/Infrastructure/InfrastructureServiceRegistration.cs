using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.AI;
using Infrastructure.Pdf;
using Infrastructure.Qdrant_VectorDB;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI.VectorStores;
using Qdrant.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var DefaultConnection = configuration.GetConnectionString("DefaultConnection");
            var qdrantConfig = configuration.GetSection("Qdrant");
            var openAIConfig = configuration.GetSection("OpenAI");

            services.AddScoped<IDocumentRepository, DocumentRepository>();

            services.AddScoped<IVectorSearchService, QdrantVectorSearchService>();

            services.AddScoped<IChatService, OpenAIChatService>();

            services.AddScoped<IVectorStore, QdrantVectorStore>();

            services.AddScoped<IEmbeddingService, EmbeddingService>();

            services.AddScoped<IPdfExtractionService, PdfExtractionService>();

            services.AddScoped<ITextChunkingService, TextChunkingService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(DefaultConnection);
            });

            services.AddSingleton<QdrantClient>(sp =>
            {
                var host = qdrantConfig["Host"]!;
                var port = qdrantConfig["Port"]!;
                return new QdrantClient(host, int.Parse(port));
            });

            services.AddSingleton<EmbeddingClient>(sp =>
            {
                var embeddingModel = openAIConfig["EmbeddingModel"]!;
                var apiKey = openAIConfig["ApiKey"]!;
                return new EmbeddingClient(
                    model: embeddingModel,
                    apiKey: apiKey
                );
            });

            services.AddSingleton<ChatClient>(sp =>
            {
                var apiKey = openAIConfig["ApiKey"]!;
                var model = openAIConfig["EmbeddingModel"]!;
                return new ChatClient(model, apiKey);
            });
        }
    }
}
