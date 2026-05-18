using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduGestor.Migrations
{
    /// <inheritdoc />
    public partial class DisciplineAndGradeChanges1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Grades",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Grades",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Disciplines",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Disciplines",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Disciplines");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Disciplines");
        }
    }
}
