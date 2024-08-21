using System.Linq.Expressions;

using LinqKit;

using Scheduler.Contracts;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.BackgroundJobs;

public class MainReminderScheduler(
    IReminderSchedulerRepo reminderSchedulerRepo,
    INotificationService notificationService,
    ILogger<ReminderScheduler> logger)
{
    public async Task ExecuteReminderSchedulers()
    {
        foreach (var reminder in GetValidReminderSchedulers())
        {
            logger.LogInformation($"Start reminder schedulers: {reminder.Id}");
            await TryExecuteReminderSchedulersAsync(reminder);
            logger.LogInformation($"End reminder schedulers: {reminder.Id}");
        }
    }
    private async Task TryExecuteReminderSchedulersAsync(ReminderScheduler reminderScheduler)
    {
        try
        {
            await reminderSchedulerRepo.UpdateStatusBasedAsync(reminderScheduler.Id, Status.InProgress);
            await ExecuteReminderSchedulersAsync(reminderScheduler);
        }
        catch (Exception ex)
        {
            await reminderSchedulerRepo.UpdateStatusBasedAsync(reminderScheduler.Id, Status.Failed, ex.Message);
            logger.LogError(ex, "Error in TryExecuteReminderSchedulersAsync");
        }
    }

    private async Task ExecuteReminderSchedulersAsync(ReminderScheduler reminderScheduler)
        => await notificationService.SendNotificationMessageAsync(reminderScheduler);

    private IQueryable<ReminderScheduler> GetValidReminderSchedulers()
    {
        // Define individual predicates
        var notExecutedPredicate = PredicateBuilder.New<ReminderScheduler>(true)
            .And(IsNotExecutedAtAll)
            .Or(IsNotExecutedToday);

        var frequencyAndTimePredicate = PredicateBuilder.New<ReminderScheduler>(true)
            .And(IsValidForFrequencyExecution)
            .And(IsValidTimeForExecution);

        var executionPredicate = PredicateBuilder.New<ReminderScheduler>(true)
            .And(IsValidForDailyExecution)
            .Or(IsValidDayOfMonthForExecution
                .And(IsValidForWeeklyExecution
                .Or(IsValidForYearlyExecution)));

        // Combine all the predicates
        var predicate = notExecutedPredicate
            .And(frequencyAndTimePredicate)
            .And(executionPredicate);

        return reminderSchedulerRepo
            .GetUnDeletedNoTracking()
            .AsExpandable()
            .Where(predicate);
    }

    private readonly Expression<Func<ReminderScheduler, bool>> IsNotExecutedAtAll =
    reminder => !reminder.ExecutedAt.HasValue;

    private readonly Expression<Func<ReminderScheduler, bool>> IsNotExecutedToday =
    reminder => reminder.ExecutedAt!.Value.Date != DateTime.UtcNow.Date;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidTimeForExecution =
    reminder => reminder.Time.HasValue
                            && reminder.Time.Value.Hour == DateTime.UtcNow.Hour;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidDayOfMonthForExecution =
    reminder => reminder.Time.HasValue
                            && reminder.Time.Value.Hour == DateTime.UtcNow.Hour;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidForDailyExecution =
    reminder => reminder.Frequency == Frequency.Daily;


    private readonly Expression<Func<ReminderScheduler, bool>> IsValidForFrequencyExecution =
    reminder => reminder.Frequency != Frequency.Non;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidForWeeklyExecution =
    reminder => reminder.Frequency == Frequency.Weekly
                            && reminder.Day.HasValue
                            && reminder.Day.Value == DateTime.UtcNow.DayOfWeek;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidForMonthlyExecution =
    reminder => reminder.Frequency == Frequency.Monthly;

    private readonly Expression<Func<ReminderScheduler, bool>> IsValidForYearlyExecution =
    reminder => reminder.Frequency == Frequency.Yearly
                           && reminder.Month.HasValue
                           && (int)reminder.Month == DateTime.UtcNow.Month;


}
