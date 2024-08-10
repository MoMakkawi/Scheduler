using Hangfire;
using Hangfire.MySql;

using Microsoft.EntityFrameworkCore;

using Scheduler.Data;

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


app.Run();
