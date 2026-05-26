using Microsoft.EntityFrameworkCore;
using Oficios.Abstractions;
using Oficios.DataAccess;

namespace OficiosDataAccess
{
    public class DbContext<T> : IDbContext<T> where T : class, IEntity
    {
        DbSet<T> _Items;
        DbDataAccess _ctx;
        public DbContext(DbDataAccess ctx)
        {
            _ctx = ctx;
            _Items = ctx.Set<T>();
        }
        public void Delete(int id)
        {
            var entity = _Items.FirstOrDefault(e => e.Id == id);

        }

        public IList<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public T Save(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
