using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduGestor.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Guardians",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_UserId",
                table: "Guardians",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guardians_AspNetUsers_UserId",
                table: "Guardians",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guardians_AspNetUsers_UserId",
                table: "Guardians");

            migrationBuilder.DropIndex(
                name: "IX_Guardians_UserId",
                table: "Guardians");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Guardians");
        }
    }
}
