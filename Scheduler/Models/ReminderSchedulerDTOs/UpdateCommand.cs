using Scheduler.Enums;

namespace Scheduler.Models.ReminderSchedulerDTOs;

public record UpdateCommand(
    int Id,
    string Name,
    string? Description,
    Frequency Frequency,
    int? Year,
    MonthOfYear? Month,
    int? DayNumber,
    DayOfWeek? Day,
    TimeOnly? Time);