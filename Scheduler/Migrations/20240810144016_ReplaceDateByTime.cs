using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Scheduler.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDateByTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ReminderSchedulers");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Time",
                table: "ReminderSchedulers",
                type: "time(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "ReminderSchedulers");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "ReminderSchedulers",
                type: "date",
                nullable: true);
        }
    }
}
