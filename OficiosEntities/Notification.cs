using Oficios.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Oficios.Entities
{
    public class Notification : IEntity
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }
        public virtual Worker Worker { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public bool IsRead { get; set; } = false;
    }
}


