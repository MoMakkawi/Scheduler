using System.Linq.Expressions;

using LinqKit;

using Scheduler.Contracts;
using Scheduler.Entities;
using Scheduler.Enums;

namespace Scheduler.BackgroundJobs;

public class MainReminderScheduler(IReminderSchedulerRepo reminderSchedulerRepo)
{
    public async Task ExecuteReminderSchedulers()
    {
        var reminderSchedulers = GetValidReminderSchedulers();
        foreach (var reminder in reminderSchedulers)
        {
            
            Console.WriteLine();
        }
    }
    private async Task TryExecuteReminderSchedulersAsync(ReminderScheduler reminderScheduler)
    {
        try
        {
            reminderSchedulerRepo.UpdateForLastExactionAsync()
        }
        catch (Exception)
        {

            throw;
        }
    }
    private async Task ExecuteReminderSchedulersAsync(ReminderScheduler reminderScheduler)
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    }

    private IQueryable<ReminderScheduler> GetValidReminderSchedulers()
    {
        #region Base predicates

        var basePredicate = PredicateBuilder.New<ReminderScheduler>(true)
             .And(IsNotExecutedAtAll)
             .Or(IsNotExecutedToday);
        #endregion

        #region Combine all frequency-specific predicates

        var frequencyPredicate = PredicateBuilder.New<ReminderScheduler>(true)
            .And(IsValidForFrequencyExecution)
            .And(IsValidTimeForExecution)

            .And(IsValidForDailyExecution)
            .Or(IsValidForWeeklyExecution)

            .Or(IsValidForMonthlyExecution.And(IsValidDayOfMonthForExecution))
            .Or(IsValidForYearlyExecution.And(IsValidDayOfMonthForExecution));
        #endregion

        // Combine base and frequency predicates
        var predicate = basePredicate.And(frequencyPredicate);

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
