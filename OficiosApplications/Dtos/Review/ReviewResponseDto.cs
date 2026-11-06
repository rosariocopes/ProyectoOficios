using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public int Score { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string AuthorName { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string AuthorRole { get; set; } = null!;
    }
}
