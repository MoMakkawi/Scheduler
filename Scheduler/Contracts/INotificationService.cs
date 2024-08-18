using Scheduler.Entities;

namespace Scheduler.Contracts;

public interface INotificationService
{
    Task SendNotificationMessageAsync(JobScheduler scheduler);
}
