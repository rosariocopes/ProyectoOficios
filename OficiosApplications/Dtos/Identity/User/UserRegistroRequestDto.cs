using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Identity.User
{
    public class UserRegistroRequestDto
    {
    
            [Required]
            public string Nombres { get; set; }
            [Required]
            public string Apellidos { get; set; }
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            [Required]
            public string Password { get; set; }
            [DataType(DataType.Date)]
            public DateTime? FechaNacimiento { get; set; }
        }
    }


