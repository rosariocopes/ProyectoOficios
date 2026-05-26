using Oficios.Abstractions;

namespace Oficios.Repository
{
    public interface IRepository<T> : IDbOperation<T>
    {

    }
    public class Repository<T> : IRepository<T> where T : class
    {
        IDbContext <T> _dbContext;
        public Repository(IDbContext<T> dbContext)
        {
            _dbContext = dbContext;
        }
        public void Delete(int id)
        {
            _dbContext.Delete(id);
        }

        public IList<T> GetAll()
        {
            return _dbContext.GetAll();
        }

        public T GetById(int id)
        {
            return _dbContext.GetById(id);
        }

        public T Save(T entity)
        {
            return _dbContext.Save(entity);
        }
    }
}
