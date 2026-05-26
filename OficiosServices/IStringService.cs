using System;
using System.Collections.Generic;
using System.Text;

namespace Oficios.Services
{
    public interface IStringService
    {
        string GetCompleteName(string name, string lastName);
    }

    public class StringService : IStringService
    {
        public string GetCompleteName(string name, string lastName)
        {
            return string.Join(" ", name, lastName);
        }
    }
}
