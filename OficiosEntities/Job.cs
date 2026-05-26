using Oficios.Abstractions;
using Oficios.Enums;
using OficiosEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oficios.Entities
{
    public class Job : IEntity
    {
        //trabajo que el cliente necesita realizar, con un título,
        //descripción, dirección, presupuesto estimado y fecha de creación.
        //El cliente puede recibir propuestas de los trabajadores para realizar el trabajo.
        public Job()
        {
            Proposals = new HashSet<Proposal>();
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal EstimatedBudget { get; set; }
        public DateTime CreatedDate { get; set; }
       public JobStatus Status { get; set; }
        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; } = null!;
        public virtual ICollection<Proposal> Proposals { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }

    }
}
