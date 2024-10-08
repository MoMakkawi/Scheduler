﻿using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

using Scheduler.Entities;
using Scheduler.Data;
using Scheduler.Contracts;

namespace Scheduler.Repositories;

public class GenericRepo<T>(MySQLDBContext dbContext) : IGenericRepo<T> where T : Entity
{
    // Add single entity or group.
    public virtual async Task<T> AddAsync(T entity)
    {
        await dbContext.Set<T>().AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await dbContext.Set<T>().AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    // Update single entity or group.
    public virtual async Task<T> UpdateAsync(T entity)
    {
        dbContext.Entry(entity).State = EntityState.Detached;
        dbContext.Set<T>().Update(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<ICollection<T>> UpdateRangeAsync(ICollection<T> entities)
    {
        foreach (var entity in entities)
        {
            dbContext.Entry(entity).State = EntityState.Detached;
            dbContext.Set<T>().Update(entity);
        }
        await dbContext.SaveChangesAsync();
        return entities;
    }

    // (un)soft delete single entity or group.
    public virtual async Task UnSoftDeleteAsync(T entity)
    {
        entity.IsDeleted = false;
        await UpdateAsync(entity);
    }

    public virtual async Task SoftDeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        await UpdateAsync(entity);
    }
    public virtual async Task SoftDeleteRangeAsync(ICollection<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            dbContext.Set<T>().Update(entity);
        }

        await dbContext.SaveChangesAsync();
    }

    // Hard delete single entity or group.
    public virtual async Task HardDeleteAsync(T entity)
    {
        dbContext.Set<T>().Remove(entity);
        await dbContext.SaveChangesAsync();
    }
    public virtual async Task HardDeleteRangeAsync(ICollection<T> entities)
    {
        dbContext.Set<T>().RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }

    // Existing functions
    public virtual async Task<bool> IsExistAsync(int id)
        => await dbContext.Set<T>().FindAsync(id) is not null;
    public virtual async Task<bool> IsExistUnDeletedAsync(int id)
        => await dbContext.Set<T>().AnyAsync(entity => entity.Id == id && !entity.IsDeleted);

    // Get entity
    public virtual async Task<T> GetByIdAsync(int id)
        => await dbContext.Set<T>().FindAsync(id)
        ?? throw new NullReferenceException($"There no {typeof(T).Name} by id: {id}");

    // Get as List
    public async Task<IReadOnlyList<T>> GetListAsync() => await dbContext
        .Set<T>()
        .ToListAsync();
    public async Task<IReadOnlyList<T>> PaganeAsync(int pageNumber, int pageSize, bool withDeleted = false) => await dbContext
        .Set<T>()
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .AsNoTracking()
        .OrderByDescending(x => x.CreatedAt)
        .Where(x => x.IsDeleted == withDeleted)
        .ToListAsync();

    // Get All "Deleted + UnDeleted" records tracking and no tracking.
    public virtual IQueryable<T> GetAllNoTracking() => dbContext
        .Set<T>()
        .AsNoTracking()
        .OrderByDescending(x => x.CreatedAt);
    public virtual IQueryable<T> GetAllTracking() => dbContext
        .Set<T>()
        .OrderByDescending(x => x.CreatedAt);

    // Get "UnDeleted" records tracking and no tracking.
    public virtual IQueryable<T> GetUnDeletedNoTracking() => dbContext
        .Set<T>()
        .AsNoTracking()
        .Where(x => !x.IsDeleted)
        .OrderByDescending(x => x.CreatedAt);

    public virtual IQueryable<T> GetUnDeletedTracking() => dbContext
        .Set<T>()
        .Where(x => !x.IsDeleted)
        .OrderByDescending(x => x.CreatedAt);

    // Get "Soft Deleted" records tracking and no tracking.
    public virtual IQueryable<T> GetSoftDeletedNoTracking() => dbContext
        .Set<T>()
        .AsNoTracking()
        .Where(x => x.IsDeleted)
        .OrderByDescending(x => x.CreatedAt);

    public virtual IQueryable<T> GetSoftDeletedTracking() => dbContext
        .Set<T>()
        .AsNoTracking()
        .Where(x => x.IsDeleted)
        .OrderByDescending(x => x.CreatedAt);

    // For transaction
    public IDbContextTransaction BeginTransaction() => dbContext.Database.BeginTransaction();

    public void Commit() => dbContext.Database.CommitTransaction();
    public void RollBack() => dbContext.Database.RollbackTransaction();
    public async Task SaveChangesAsync() => await dbContext.SaveChangesAsync();

    // sql statements 
    public void ExecuteSQL(string query, params object[] agrs) => dbContext.Database.ExecuteSqlRaw(query, agrs);
}
