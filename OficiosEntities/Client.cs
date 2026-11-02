using Oficios.Abstractions;
using Oficios.Entities;
using Oficios.Entities.MicrosoftEntity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OficiosEntities
{
    public class Client : IEntity
    {
        //cliente que necesita que le realicen un trabajo

        public Client()
        {
            Jobs = new HashSet<Job>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual User User { get; set; } = null!;

        [StringLength(30)]
        public string Name { get; set; } = null!;

        [StringLength(30)]
        public string LastName { get; set; } = null!;

        public string Dni { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public virtual ICollection<Job> Jobs { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}


