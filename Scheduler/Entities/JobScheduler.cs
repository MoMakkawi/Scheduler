using Scheduler.Enums;

namespace Scheduler.Entities;

public class JobScheduler : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
    public Status Status { get; set; }

    public Frequency Frequency { get; set; }
    public int? Year { get; set; }
    public MonthOfYear? Month { get; set; }
    public int? DayNumber { get; set; }
    public DayOfWeek? Day { get; set; }
    public TimeOnly? Time { get; set; }
}
