using Oficios.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oficios.Repository
{
   public interface IRepository<T> : IDbOperation<T>
    {
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        IDbContext<T> _dbcontext;
        public Repository(IDbContext<T> dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public void Delete(int id)
        {
            _dbcontext.Delete(id);
        }

        public IList<T> GetAll()
        {
            return _dbcontext.GetAll();
        }

        public T GetById(int id)
        {
            return _dbcontext.GetById(id);
        }

        public T Save(T entity)
        {
            return _dbcontext.Save(entity);
        }
    }


}
