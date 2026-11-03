using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Applications.Dtos.Login
{
    public class LoginUserResponseDto
    {
        public string Token { get; set; }
        public string? UserName { get; set; }
        public string? Mail { get; set; }
        public string? Role { get; set; } 
        public bool Login { get; set; }
        public List<string> Errores { get; set; }
    }
}
