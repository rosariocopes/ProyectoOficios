using Oficios.Abstractions;
using OficiosEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Oficios.Entities
{
    public class Worker : IEntity
    {
        //trabajador que ofrece servicios
        public Worker()
        {
            Proposals = new HashSet<Proposal>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }

        public string? License { get; set; }

        public string Description { get; set; } = null!;

        public string City { get; set; } = null!;

        [ForeignKey(nameof(Profession))]
        public int ProfessionId { get; set; }

        public virtual Profession Profession { get; set; } = null!;

        public virtual ICollection<Proposal> Proposals { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
