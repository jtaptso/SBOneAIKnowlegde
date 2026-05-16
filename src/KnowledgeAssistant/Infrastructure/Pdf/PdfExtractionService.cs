using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace Infrastructure.Pdf
{
    public class PdfExtractionService : IPdfExtractionService
    {
        public async Task<string> ExtractTextAsync(string filePath)
        {
            if(!File.Exists(filePath))
                throw new FileNotFoundException("PDF file not found", filePath);

            return await Task.Run(() =>
            {
                using var document = PdfDocument.Open(filePath);

                var text = new System.Text.StringBuilder();

                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
                return text.ToString();
            });
        }

    }
}
