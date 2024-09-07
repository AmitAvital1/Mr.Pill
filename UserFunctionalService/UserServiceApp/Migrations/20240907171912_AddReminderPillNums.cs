using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderPillNums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "numOfPills",
                table: "Reminders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

                migrationBuilder.Sql("UPDATE Reminders SET numOfPills = 1 WHERE numOfPills IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numOfPills",
                table: "Reminders");
        }
    }
}
