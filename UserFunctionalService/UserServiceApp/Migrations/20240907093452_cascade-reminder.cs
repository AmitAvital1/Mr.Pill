using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class cascadereminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_UserMedications_UserMedicationId",
                table: "Reminders");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_UserMedications_UserMedicationId",
                table: "Reminders",
                column: "UserMedicationId",
                principalTable: "UserMedications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_UserMedications_UserMedicationId",
                table: "Reminders");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_UserMedications_UserMedicationId",
                table: "Reminders",
                column: "UserMedicationId",
                principalTable: "UserMedications",
                principalColumn: "Id");
        }
    }
}
