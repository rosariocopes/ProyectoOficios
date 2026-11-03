using Oficios.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Job
{
    public class JobResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal EstimatedBudget { get; set; }
        public DateTime CreatedDate { get; set; }
        public JobStatus Status { get; set; }
        public string Profession { get; set; } = null!;
        public string Client { get; set; } = null!;
    }
}
