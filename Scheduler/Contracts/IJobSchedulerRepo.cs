using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Contracts;

public interface IJobSchedulerRepo<Scheduler> : IGenericRepo<Scheduler> where Scheduler : JobScheduler
{
    Task<IReadOnlyList<Scheduler>> GetListByStatusAsync(Status status);
    Task<Scheduler> UpdateStatusBasedAsync(int id, Status status, string? errorMessage = null);
}
