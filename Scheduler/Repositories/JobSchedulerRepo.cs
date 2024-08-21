using Microsoft.EntityFrameworkCore;

using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Repositories;

public class JobSchedulerRepo<Scheduler>(MySQLDBContext dbContext) : GenericRepo<Scheduler>(dbContext), IJobSchedulerRepo<Scheduler> where Scheduler : JobScheduler
{
    public async Task<IReadOnlyList<Scheduler>> GetListByStatusAsync(Status status) => await dbContext
        .Set<Scheduler>()
        .Where(x => !x.IsDeleted && x.Status == status)
        .ToListAsync();

    public async Task<Scheduler> UpdateStatusBasedAsync(int id, Status status, string? errorMessage = null)
    {
        var jobScheduler = await GetByIdAsync(id);

        jobScheduler.Status = status;

        switch (status)
        {
            case Status.InProgress: break; 
            case Status.Success:
                {
                    jobScheduler.ExecutedAt = DateTime.UtcNow;
                    break;
                }
            case Status.Failed:
                {
                    jobScheduler.ErrorMessage = errorMessage!;
                    break;
                }
            case Status.New: throw new ArgumentException("Scheduler will be new only when you create it.");
            default: throw new NotImplementedException($"There no Update Job Scheduler implementation for {status.ToString()}");
        }

        return await UpdateAsync(jobScheduler);
    }
}
