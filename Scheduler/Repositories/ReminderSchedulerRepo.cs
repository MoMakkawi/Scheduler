using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;

namespace Scheduler.Repositories;

public class ReminderSchedulerRepo(MySQLDBContext dbContext) : JobSchedulerRepo<ReminderScheduler>(dbContext), IReminderSchedulerRepo
{
    public async Task PrintConsoleAsync(ReminderScheduler scheduler)
    {
        try
        {
            Console.WriteLine($"start remaindering {scheduler.Id}");

            string remainderingMessage = $"{scheduler.Id}: Please don't forget {scheduler.Name}";
            if (scheduler.Description is not null) remainderingMessage += $" to {scheduler.Description}";

            await UpdateForLastExactionAsync(scheduler.Id);

            Console.WriteLine(remainderingMessage);
        }
        catch(Exception ex) 
        {
            await UpdateForLastExactionAsync(scheduler.Id, IsSuccess: false, ex.Message);
        }
    }
}
