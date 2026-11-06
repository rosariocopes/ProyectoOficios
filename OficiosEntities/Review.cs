using Oficios.Abstractions;
using Oficios.Entities.MicrosoftEntity;
using OficiosEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Oficios.Entities
{
    public class Review : IEntity
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedDate { get; set; }

        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }

        [ForeignKey(nameof(Author))]
        public Guid AuthorUserId { get; set; }

        public virtual Job Job { get; set; } = null!;
        public virtual Client Client { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
    }
}

