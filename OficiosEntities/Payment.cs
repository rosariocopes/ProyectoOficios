using Oficios.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Oficios.Entities
{
    public class Payment : IEntity
    {
        public int Id { get; set; }
        public decimal Amount { get; set; } //pago parcial o total del trabajo, dependiendo de lo acordado entre el cliente y el trabajador.
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; } = null!;

        [ForeignKey(nameof(Job))]
        public int JobId { get; set; }
        [ForeignKey(nameof(PaymentType))]
        public int PaymentTypeId { get; set; }
        public virtual Worker Job { get; set; } = null!;
        public virtual PaymentType PaymentType { get; set; } = null!;
    }
}