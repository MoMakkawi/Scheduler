using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Contracts;

public interface IJobSchedulerRepo<T> where T : JobScheduler
{
    public Task<IReadOnlyList<T>> GetListByStatusAsync(Status status);
    public Task<T> UpdateForLastExactionAsync(int id, bool IsSuccess = true, string? ErrorMessage = null);
}
