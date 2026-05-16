using Application.Interfaces;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.AI
{
    public class OpenAIChatService : IChatService
    {
        private readonly ChatClient _chatClient;
        private readonly IVectorSearchService _searchService;

        public OpenAIChatService(ChatClient chatClient, IVectorSearchService searchService)
        {
            _chatClient = chatClient;
            _searchService = searchService;
        }

        public async Task<string> AskAsync(string question)
        {
            // Retrieve relevant chunks
            var results = await _searchService.SearchAsync(question);

            // Build context
            var context = string.Join(
                "\n\n",
                results.Select(r => r.Content)
                );

            // Build prompt
            var prompt = $"""
                You are an SAP Business One AI assistant.
                Answer the user's question ONLY using the provided context.
                If the answer is not in the context, say:
                "I could not find the answer in the knowledge base."

                context: {context}

                Question: {question}
                """;

            // Ask GPT
            var response = await _chatClient.CompleteChatAsync(prompt);

            return response.Value.Content[0].Text;

        }
    }
}
