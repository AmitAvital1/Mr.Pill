using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class makemedicatongeneric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrugEnglishName",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DrugHebrewName",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "EnglishDescription",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "HebrewDescription",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Medications");

            migrationBuilder.AddColumn<string>(
                name: "DrugEnglishName",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrugHebrewName",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnglishDescription",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HebrewDescription",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrugEnglishName",
                table: "MedicationRepos");

            migrationBuilder.DropColumn(
                name: "DrugHebrewName",
                table: "MedicationRepos");

            migrationBuilder.DropColumn(
                name: "EnglishDescription",
                table: "MedicationRepos");

            migrationBuilder.DropColumn(
                name: "HebrewDescription",
                table: "MedicationRepos");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "MedicationRepos");

            migrationBuilder.AddColumn<string>(
                name: "DrugEnglishName",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrugHebrewName",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnglishDescription",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HebrewDescription",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
