using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oficios.Abstractions
{
    public interface ITokensParameters
    {
        string UserName { get; set; }
        string Email { get; set; }
        string PaswordHash { get; set; }
        string Id { get; set; }
        IList<string>? Roles { get; set; }
    }
}
