using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scheduler.Contracts;
using Scheduler.Entities;
using Scheduler.Enums;
using Scheduler.Models.ReminderSchedulerDTOs;

namespace Scheduler.Endpoints;

public class RemainderSchedulerEndpoints : IEndpointDefinition
{
    public string prefix = "scheduler/remainder";
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet(prefix, async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo)
            => await reminderSchedulerRepo.GetAllNoTracking().ToListAsync());
        app.MapGet(prefix + "/undeleted", async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, Status status)
            => await reminderSchedulerRepo.GetListByStatusAsync(status));
        app.MapGet(prefix + "/id", async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, int id)
            => await reminderSchedulerRepo.GetByIdAsync(id));

        app.MapDelete(prefix, async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, int id) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(id);
            await reminderSchedulerRepo.SoftDeleteAsync(reminderScheduler);
        });

        app.MapPatch(prefix + "/un-delete", async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, int id) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(id);
            await reminderSchedulerRepo.UnSoftDeleteAsync(reminderScheduler);
        });

        app.MapPost(prefix, async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, CreateCommand createCommand) =>
        {
            var reminderScheduler = new ReminderScheduler()
            {
                Day = createCommand.Day,
                DayNumber = createCommand.DayNumber,
                Description = createCommand.Description,
                Frequency = createCommand.Frequency,
                Month = createCommand.Month,
                Name = createCommand.Name,
                Time = createCommand.Time,
                Year = createCommand.Year,
                
            };
            return await reminderSchedulerRepo.AddAsync(reminderScheduler);
        });

        app.MapPut(prefix, async ([FromServices] IReminderSchedulerRepo reminderSchedulerRepo, UpdateCommand updateCommand) =>
        {
            var reminderScheduler = await reminderSchedulerRepo.GetByIdAsync(updateCommand.Id);

            reminderScheduler.Day = updateCommand.Day;
            reminderScheduler.DayNumber = updateCommand.DayNumber;
            reminderScheduler.Description = updateCommand.Description;
            reminderScheduler.Frequency = updateCommand.Frequency;
            reminderScheduler.Month = updateCommand.Month;
            reminderScheduler.Name = updateCommand.Name;
            reminderScheduler.Time = updateCommand.Time;
            reminderScheduler.Year = updateCommand.Year;

            return await reminderSchedulerRepo.UpdateAsync(reminderScheduler);
        });
    }
}
