using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductsOrders.DAL.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<TEntity> Create(TEntity obj);

        Task Update(string id, TEntity obj);

        void Delete(string id);

        Task<TEntity> Get(string id);

        Task<IEnumerable<TEntity>> Get();
    }
}