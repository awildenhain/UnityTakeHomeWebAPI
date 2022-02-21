using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnityTakeHomeWebAPI.Migrations
{
    public partial class ConvertingToTimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Messages");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                table: "Messages",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Messages");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Messages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
