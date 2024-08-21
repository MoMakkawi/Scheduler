using Scheduler.Contracts;
using Scheduler.Data;
using Scheduler.Entities;

namespace Scheduler.Repositories;

public class ReminderSchedulerRepo(MySQLDBContext dbContext) 
    : JobSchedulerRepo<ReminderScheduler>(dbContext)
    , IReminderSchedulerRepo;
