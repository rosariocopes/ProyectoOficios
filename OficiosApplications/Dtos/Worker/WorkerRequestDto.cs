using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Worker
{
    public class WorkerRequestDto
    {
        [StringLength(30)]
        public string Name { get; set; } = null!;
        [StringLength(30)]
        public string LastName { get; set; } = null!;
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;
        public string? License { get; set; }
        public string City { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProfessionId { get; set; }
    }
}
