using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduGestor.Migrations
{
    /// <inheritdoc />
    public partial class AddChanges05 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Grades");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Attendances",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Bimester",
                table: "Attendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bimester",
                table: "Attendances");

            migrationBuilder.AddColumn<decimal>(
                name: "Frequency",
                table: "Grades",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Attendances",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
