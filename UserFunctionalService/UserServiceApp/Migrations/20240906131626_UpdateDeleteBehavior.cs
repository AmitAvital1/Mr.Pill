using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMedications_MedicineCabinets_MedicineCabinetId",
                table: "UserMedications");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedications_MedicineCabinets_MedicineCabinetId",
                table: "UserMedications",
                column: "MedicineCabinetId",
                principalTable: "MedicineCabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMedications_MedicineCabinets_MedicineCabinetId",
                table: "UserMedications");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedications_MedicineCabinets_MedicineCabinetId",
                table: "UserMedications",
                column: "MedicineCabinetId",
                principalTable: "MedicineCabinets",
                principalColumn: "Id");
        }
    }
}
