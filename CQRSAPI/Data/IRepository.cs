using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSAPI.Data
{

    public interface IRepository<T>
    {

        Task<T> AddAsync(
            T item, 
            CancellationToken cancellationToken);

        Task<int> DeleteAsync(
            int id, 
            CancellationToken cancellationToken);

        Task<int> UpdateAsync(
            T item, 
            CancellationToken cancellationToken);

        Task<T> FindAsync(
            int id, 
            CancellationToken cancellationToken);

        Task<List<T>> FindAllAsync(
            int pageSize, 
            int pageNumber,
            List<KeyValuePair<string, string>> queryParams,
            CancellationToken cancellationToken);

        Task<bool> Exists(
            int id, 
            CancellationToken cancellationToken);

    }

}
