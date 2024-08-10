using Microsoft.EntityFrameworkCore;

using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Repositories;

public class JobSchedulerRepo<T>(MySQLDBContext dbContext) : GenericRepo<T>(dbContext), IJobSchedulerRepo<T> where T : JobScheduler
{
    public async Task<IReadOnlyList<T>> GetListByStatusAsync(Status status) => await dbContext
        .Set<T>()
        .Where(x => !x.IsDeleted && x.Status == status)
        .ToListAsync();

    public async Task<T> UpdateForLastExactionAsync(int id, bool IsSuccess = true, string? ErrorMessage = null)
    {
        var jobScheduler = await GetByIdAsync(id);

        if (IsSuccess) 
        {
            jobScheduler.Status = Status.Success;
            jobScheduler.ExecutedAt = DateTime.UtcNow;
        }
        else
        {
            jobScheduler.Status = Status.Failed;
            jobScheduler.ErrorMessage = ErrorMessage!;
        }

        return await UpdateAsync(jobScheduler);
    }
}
