using Scheduler.Contracts;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.Services;

public class NotificationService(ILogger<NotificationService> logger, IJobSchedulerRepo<JobScheduler> schedulerRepo) : INotificationService
{
    public async Task SendNotificationMessageAsync(JobScheduler scheduler)
    {
        try
        {
            logger.LogInformation($"send remaindering notification.");

            string remainderingMessage = $"remaindering scheduler {scheduler.Id}: Please don't forget {scheduler.Name}";
            if (scheduler.Description is not null) remainderingMessage += $" to {scheduler.Description}";

            await schedulerRepo.UpdateStatusBasedAsync(scheduler.Id, Status.Success);

            // send notification as print it as warning message
            logger.LogWarning(remainderingMessage);

            logger.LogInformation($"remaindering warning received.");
        }
        catch (Exception ex)
        {
            await schedulerRepo.UpdateStatusBasedAsync(scheduler.Id, Status.Failed, ex.Message);
            logger.LogError(ex, "Error in SendNotificationMessageAsync.");
        }
    }
}
