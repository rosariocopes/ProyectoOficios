using Oficios.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oficios.Entities
{
    public class PaymentType : IEntity
    {
        public PaymentType()
        {
            Payments = new HashSet<Payment>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Payment> Payments { get; set;}
    }
}
