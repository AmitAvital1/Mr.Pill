using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMergeOrNotMergeTwoHouses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MergeToNewHouse",
                table: "HouseRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MergeToNewHouse",
                table: "HouseRequests");
        }
    }
}
