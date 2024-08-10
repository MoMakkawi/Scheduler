﻿using Scheduler.Entities;

namespace Scheduler.Contracts;

public interface IReminderSchedulerRepo : IJobSchedulerRepo<ReminderScheduler>
{
    public Task PrintConsoleAsync(ReminderScheduler scheduler);
}
