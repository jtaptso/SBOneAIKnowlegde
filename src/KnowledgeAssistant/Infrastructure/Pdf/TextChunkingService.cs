using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Pdf
{
    public class TextChunkingService : ITextChunkingService
    {
        public List<string> ChunkText(string text, int size = 800)
        {
            var chunks = new List<string>();
            for (int i = 0; i < text.Length; i+=size)
            {
                int length = Math.Min(size, text.Length - i);
                chunks.Add(text.Substring(i, length));
            }

            return chunks;
        }
    }
}
