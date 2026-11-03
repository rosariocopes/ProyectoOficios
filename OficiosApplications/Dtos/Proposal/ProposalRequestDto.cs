using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Proposal
{
    public class ProposalRequestDto
    {
        public int JobId { get; set; }
        public decimal LaborCost { get; set; }
        public decimal EstimatedMaterialsCost { get; set; }
        public string Message { get; set; } = null!;
    }
}
