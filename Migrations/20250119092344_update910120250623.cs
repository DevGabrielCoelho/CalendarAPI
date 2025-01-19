using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendarAPI.Migrations
{
    /// <inheritdoc />
    public partial class update910120250623 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeBefore",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "SendedEmail",
                table: "Events");

            migrationBuilder.AddColumn<double>(
                name: "MinutesBefore",
                table: "Reminders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "SendedEmail",
                table: "Reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutesBefore",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "SendedEmail",
                table: "Reminders");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeBefore",
                table: "Reminders",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "SendedEmail",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
