using System.Linq;
using System.Threading.Tasks;

namespace RUG.WebEng.Transactions.Repositories;

public interface IRepository<T, TKey> where T : class
{
    Task<bool> CreateAsync(T item);
    Task DeleteAsync(T item);
    Task<bool> UpdateAsync(T item);
    Task<T?> FindAsync(TKey key);
    IQueryable<T> SimpleCollection { get; }
    IQueryable<T> FullCollection { get; }
}