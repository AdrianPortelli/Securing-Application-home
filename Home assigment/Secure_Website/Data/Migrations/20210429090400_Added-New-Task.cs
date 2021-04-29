using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Secure_Website.Data.Migrations
{
    public partial class AddedNewTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    TaskName = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    TeacherId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTask", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTask");
        }
    }
}
