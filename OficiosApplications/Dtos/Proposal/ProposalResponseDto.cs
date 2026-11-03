using Oficios.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Proposal
{
    public class ProposalResponseDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = null!;
        public decimal LaborCost { get; set; }
        public decimal EstimatedMaterialsCost { get; set; }
        public decimal ProposedBudget { get; set; }
        public string Message { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public ProposalStatus Status { get; set; }
    }
}
