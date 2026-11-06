using Oficios.Abstractions;
using Oficios.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oficios.Entities
{
    public class Payment : IEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = null!;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pendiente;
        public DateTime? PaymentReportedDate { get; set; }
        public DateTime? PaymentConfirmedDate { get; set; }

        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }
        [ForeignKey(nameof(Proposal))]
        public int ProposalId { get; set; }
        [ForeignKey(nameof(PaymentType))]
        public int PaymentTypeId { get; set; }
        public virtual Job Job { get; set; } = null!;
        public virtual Proposal Proposal { get; set; } = null!;
        public virtual PaymentType PaymentType { get; set; } = null!;
    }

}

