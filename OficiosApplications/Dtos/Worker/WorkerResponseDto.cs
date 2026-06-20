using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Worker
{
    public class WorkerResponseDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Dni { get; set; }
        public string Description { get; set; }
        public string? License { get; set; }
        public string Phone { get; set; }
        public string Email{ get; set; }  
        public string Profession { get; set; }
    }
}
