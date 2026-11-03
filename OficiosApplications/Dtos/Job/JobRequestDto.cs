using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Job
{
    public class JobRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal EstimatedBudget { get; set; }
        public int ProfessionId { get; set; }

    }
}
