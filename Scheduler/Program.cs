using System.Linq.Expressions;

using Hangfire;
using Hangfire.MySql;

using LinqKit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;
using Scheduler.Enums;
using Scheduler.Extensions;
using Scheduler.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("MySQLConnectionString");
builder.Services.AddDbContext<MySQLDBContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var hangfireConnectionString = builder.Configuration.GetConnectionString("HangfireConnectionString");
builder.Services.AddHangfire(config => config
    .UseStorage(new MySqlStorage(connectionString,
                    new MySqlStorageOptions
                    {
                        TablesPrefix = "HangFire"
                    })));
builder.Services.AddHangfireServer();

builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
builder.Services.AddScoped(typeof(IJobSchedulerRepo<>), typeof(JobSchedulerRepo<>));
builder.Services.AddScoped<IReminderSchedulerRepo, ReminderSchedulerRepo>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard()
    .UseHangfireServer();

app.UseHttpsRedirection();

// Register endpoints dynamically
app.AddEndpoints();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<MySQLDBContext>();


var x = new MyClass();
Console.WriteLine("________=====_______good start_____=====______");
var s = x.GetReminders(dbContext).ToList();
Console.WriteLine(s.Count);
Console.WriteLine("________====_______good end______=======_____");

Console.WriteLine("_________=======______good start____=======_______");
var sc = dbContext.Set<ReminderScheduler>().ToList().Where(x => x.ExecutedAt is null).ToList();
Console.WriteLine(sc.Count);
Console.WriteLine("________=======_______good end_____=======______");

app.Run();


class MyClass
{
    public IQueryable<ReminderScheduler> GetReminders(MySQLDBContext _context)
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

        return _context.ReminderSchedulers
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
