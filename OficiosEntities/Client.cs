using Oficios.Abstractions;
using Oficios.Entities;
using System.ComponentModel.DataAnnotations;

namespace OficiosEntities
{
    public class Client : IEntity
    {
        //cliente que necesita que le realicen un trabajo

        public Client()
        {
            Jobs = new HashSet<Worker>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }

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

        public virtual ICollection<Worker> Jobs { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}


