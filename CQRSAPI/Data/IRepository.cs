using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;

namespace CQRSAPI.Data
{

    public interface IRepository<T>
    {

        Task<T> AddAsync(
            T item, 
            CancellationToken cancellationToken);

        Task<int> DeleteAsync(
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
            CancellationToken cancellationToken);

        Task<bool> Exists(
            int id, 
            CancellationToken cancellationToken);

    }

}
