using Microsoft.EntityFrameworkCore;

using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Repositories;

public class JobSchedulerRepo(MySQLDBContext dbContext) : GenericRepo<JobScheduler>(dbContext), IJobSchedulerRepo
{
    public async Task<IReadOnlyList<JobScheduler>> GetListByStatusAsync(Status status) => await dbContext
        .Set<JobScheduler>()
        .Where(x => !x.IsDeleted && x.Status == status)
        .ToListAsync();

    public async Task<JobScheduler> UpdateForLastExactionAsync(int id, bool IsSuccess = true, string? ErrorMessage = null)
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
