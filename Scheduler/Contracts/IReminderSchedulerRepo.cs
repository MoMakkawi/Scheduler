using Scheduler.Entities;

namespace Scheduler.Contracts;

public interface IReminderSchedulerRepo : IJobSchedulerRepo
{
    public Task PrintConsoleAsync(ReminderScheduler scheduler);
}
