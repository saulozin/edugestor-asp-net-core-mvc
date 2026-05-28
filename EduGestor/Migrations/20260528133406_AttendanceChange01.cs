using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduGestor.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceChange01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_RegistrationId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_RegistrationId_DisciplineClassId_Date",
                table: "Attendances",
                columns: new[] { "RegistrationId", "DisciplineClassId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_RegistrationId_DisciplineClassId_Date",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_RegistrationId",
                table: "Attendances",
                column: "RegistrationId");
        }
    }
}
