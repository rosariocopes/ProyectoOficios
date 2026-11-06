using Oficios.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = null!;
        public PaymentStatus Status { get; set; }

        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;

        public int ProposalId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; } = null!;

        public DateTime? PaymentReportedDate { get; set; }
        public DateTime? PaymentConfirmedDate { get; set; }
    }
}
