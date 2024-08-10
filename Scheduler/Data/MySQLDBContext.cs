using Microsoft.EntityFrameworkCore;

using Scheduler.Entities;

namespace Scheduler.Data;

public class MySQLDBContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<ReminderScheduler> ReminderSchedulers { get; set; }
}
