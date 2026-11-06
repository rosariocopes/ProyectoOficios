using Oficios.Abstractions;
using Oficios.Entities.MicrosoftEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oficios.Entities
{
    public class Worker : IEntity
    {
        //trabajador que ofrece servicios

        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Dni { get; set; } = null!;
        public string? License { get; set; }
        public string Description { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string City { get; set; } = null!;

        [ForeignKey(nameof(Profession))]
        public int ProfessionId { get; set; }

        public virtual Profession Profession { get; set; } = null!;

        public virtual ICollection<Proposal> Proposals { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
         = new HashSet<Notification>();

    }
}
