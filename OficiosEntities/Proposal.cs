using Oficios.Abstractions;
using Oficios.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oficios.Entities
{
    public class Proposal : IEntity
    {
        //Presupuesto/postulación del trabajador para realizar un trabajo,
        //con un mensaje del trabajador al cliente, la fecha de creación y
        //un estado que indique si la propuesta fue aceptada o no por el cliente.
        public int Id { get; set; }

        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }
        public decimal LaborCost { get; set; }
        public decimal EstimatedMaterialsCost { get; set; }
        public decimal ProposedBudget { get; set; }
        public string Message { get; set; } = null!;
        public string? ClientResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ProposalStatus Status { get; set; }
            = ProposalStatus.Pendiente;
        public virtual Job Job { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
    }

}

