using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Review
{
    public class ReviewRequestDto
    {
        [Required]
        public int JobId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 3)]
        public string Comment { get; set; } = null!;
    }
}
