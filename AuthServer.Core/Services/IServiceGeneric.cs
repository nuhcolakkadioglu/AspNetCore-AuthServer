using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity,TDto> where TEntity:class where TDto:class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        IQueryable<Task> Where(Expression<Func<TEntity, bool>> expression);

        Task AddAsync(TEntity entity);

        void Remove(TEntity entity);

        TEntity Update(TEntity entity);
    }
}
