using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddShelfLife : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Validity",
                table: "UserMedications",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "ShelfLife",
                table: "MedicationRepos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShelfLife",
                table: "MedicationRepos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Validity",
                table: "UserMedications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
