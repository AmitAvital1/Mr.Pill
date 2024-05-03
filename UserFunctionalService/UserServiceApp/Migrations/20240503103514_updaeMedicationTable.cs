using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class updaeMedicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Medications",
                newName: "DrugHebrewName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Medications",
                newName: "ImagePath");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Medications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DrugEnglishName",
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

            migrationBuilder.AddColumn<int>(
                name: "PillSize",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DrugEnglishName",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "EnglishDescription",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "HebrewDescription",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "PillSize",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "MedicationRepos");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Medications",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "DrugHebrewName",
                table: "Medications",
                newName: "Name");
        }
    }
}
