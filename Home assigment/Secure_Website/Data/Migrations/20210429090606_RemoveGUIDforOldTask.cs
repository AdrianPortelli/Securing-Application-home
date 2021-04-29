using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Secure_Website.Data.Migrations
{
    public partial class RemoveGUIDforOldTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskModel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskModel", x => x.Id);
                });
        }
    }
}
