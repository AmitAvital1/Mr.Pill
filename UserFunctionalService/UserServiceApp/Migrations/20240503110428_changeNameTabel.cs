using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class changeNameTabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_MedicationRepos_MedicationRepoId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Users_UserId",
                table: "Medications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medications",
                table: "Medications");

            migrationBuilder.RenameTable(
                name: "Medications",
                newName: "UserMedications");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_UserId",
                table: "UserMedications",
                newName: "IX_UserMedications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_MedicationRepoId",
                table: "UserMedications",
                newName: "IX_UserMedications_MedicationRepoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMedications",
                table: "UserMedications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedications_MedicationRepos_MedicationRepoId",
                table: "UserMedications",
                column: "MedicationRepoId",
                principalTable: "MedicationRepos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedications_Users_UserId",
                table: "UserMedications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMedications_MedicationRepos_MedicationRepoId",
                table: "UserMedications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMedications_Users_UserId",
                table: "UserMedications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMedications",
                table: "UserMedications");

            migrationBuilder.RenameTable(
                name: "UserMedications",
                newName: "Medications");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedications_UserId",
                table: "Medications",
                newName: "IX_Medications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedications_MedicationRepoId",
                table: "Medications",
                newName: "IX_Medications_MedicationRepoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medications",
                table: "Medications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_MedicationRepos_MedicationRepoId",
                table: "Medications",
                column: "MedicationRepoId",
                principalTable: "MedicationRepos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Users_UserId",
                table: "Medications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
