using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scheduler.Entities;
using Scheduler.Enums;
using Scheduler.Models.ReminderSchedulerDTOs;
using Scheduler.Repositories;

namespace Scheduler.Endpoints;

public class RemainderSchedulerEndpoints : IEndpointDefinition
{
    public string prefix = "scheduler/remainder";
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(prefix, async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo)
            => await reminderSchedulerRepo.GetAllNoTrackingAsync().ToListAsync());
        app.MapGet(prefix + "/undeleted", async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, Status status)
            => await reminderSchedulerRepo.GetListByStatusAsync(status));
        app.MapGet(prefix + "/id", async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, int id)
            => await reminderSchedulerRepo.GetByIdAsync(id));

        app.MapDelete(prefix, async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, int id) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(id);
            await reminderSchedulerRepo.SoftDeleteAsync(reminderScheduler);
        });

        app.MapPatch(prefix + "/un-delete", async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, int id) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(id);
            await reminderSchedulerRepo.UnSoftDeleteAsync(reminderScheduler);
        });

        app.MapPost(prefix, async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, CreateCommand createCommand) =>
        {
            var reminderScheduler = new ReminderScheduler()
            {
                Date = createCommand.Date,
                Day = createCommand.Day,
                DayNumber = createCommand.DayNumber,
                Description = createCommand.Description,
                Frequency = createCommand.Frequency,
                Month = createCommand.Month,
                Name = createCommand.Name,
                Year = createCommand.Year,
            };
            return await reminderSchedulerRepo.AddAsync(reminderScheduler);
        });

        app.MapPut(prefix, async ([FromServices] ReminderSchedulerRepo reminderSchedulerRepo, UpdateCommand updateCommand) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(updateCommand.Id);

            reminderScheduler.Date = updateCommand.Date;
            reminderScheduler.Day = updateCommand.Day;
            reminderScheduler.DayNumber = updateCommand.DayNumber;
            reminderScheduler.Description = updateCommand.Description;
            reminderScheduler.Frequency = updateCommand.Frequency;
            reminderScheduler.Month = updateCommand.Month;
            reminderScheduler.Name = updateCommand.Name;
            reminderScheduler.Year = updateCommand.Year;

            return await reminderSchedulerRepo.UpdateAsync(reminderScheduler);
        });
    }
}
