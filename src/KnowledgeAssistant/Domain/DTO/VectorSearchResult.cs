using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class VectorSearchResult
    {
        public string Content { get; set; }
        public string DocumentId { get; set; }
        public int ChunkIndex { get; set; }
        public float Score { get; set; }
    }
}
