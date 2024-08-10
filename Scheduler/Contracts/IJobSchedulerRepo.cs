using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Contracts;

public interface IJobSchedulerRepo
{
    public Task<IReadOnlyList<JobScheduler>> GetListByStatusAsync(Status status);
    public Task<JobScheduler> UpdateForLastExactionAsync(int id, bool IsSuccess = true, string? ErrorMessage = null);
}
