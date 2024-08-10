using Microsoft.EntityFrameworkCore.Storage;

using Scheduler.Entities;

namespace Scheduler.Contracts;

public interface IGenericRepo<T> where T : Entity
{
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<ICollection<T>> UpdateRangeAsync(ICollection<T> entities);
    Task UnSoftDeleteAsync(T entity);
    Task SoftDeleteAsync(T entity);
    Task SoftDeleteRangeAsync(ICollection<T> entities);
    Task HardDeleteAsync(T entity);
    Task HardDeleteRangeAsync(ICollection<T> entities);
    Task<bool> IsExistAsync(int id);
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetListAsync();
    Task<IReadOnlyList<T>> PaganeAsync(int pageNumber, int pageSize, bool withDeleted = false);
    IQueryable<T> GetAllNoTracking();
    IQueryable<T> GetAllTracking();
    IQueryable<T> GetUnDeletedNoTracking();
    IQueryable<T> GetUnDeletedTracking();
    IQueryable<T> GetSoftDeletedNoTracking();
    IQueryable<T> GetSoftDeletedTracking();
    IDbContextTransaction BeginTransaction();
    void Commit();
    void RollBack();
    void ExecuteSQL(string query, params object[] agrs);


}
