using Oficios.Abstractions;
using OficiosEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Oficios.Entities
{
    public class Review : IEntity
    {
        //reseñas que los clientes dejan a los trabajadores luego de que se complete un trabajo, con una puntuación y un comentario
        public int Id { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; } = null!;

        [ForeignKey(nameof(Client))]
        public int ClientId { get; set; }

        [ForeignKey(nameof(Worker))]
        public int WorkerId { get; set; }
        public virtual Client Client { get; set; } = null!;
        public virtual Worker Worker { get; set; } = null!;
    }
}
