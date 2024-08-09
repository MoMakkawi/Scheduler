using Microsoft.EntityFrameworkCore;

namespace Scheduler.Data;

public class MySQLDBContext(DbContextOptions options) : DbContext(options)
{
}
