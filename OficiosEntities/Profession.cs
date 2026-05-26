using Oficios.Abstractions;

namespace Oficios.Entities
{
    public class Profession : IEntity
    {
        //oficio o profesión que un trabajador puede tener, con un nombre y una descripción
        public Profession()
        {
            Workers = new HashSet<Worker>();
        }
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public virtual ICollection<Worker> Workers { get; set; }
    }
}
