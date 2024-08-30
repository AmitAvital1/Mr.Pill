using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBrochureUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BrochurePath",
                table: "MedicationRepos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrochurePath",
                table: "MedicationRepos");
        }
    }
}
