using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServiceApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CabinetRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsHandle = table.Column<bool>(type: "bit", nullable: false),
                    IsApprove = table.Column<bool>(type: "bit", nullable: false),
                    IsSenderSeen = table.Column<bool>(type: "bit", nullable: false),
                    DateStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CabinetRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicationRepos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrugEnglishName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrugHebrewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HebrewDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationRepos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MedicineCabinets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineCabinetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineCabinets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineCabinets_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineCabinetUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    MedicineCabinetId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineCabinetUsers", x => new { x.UserId, x.MedicineCabinetId });
                    table.ForeignKey(
                        name: "FK_MedicineCabinetUsers_MedicineCabinets_MedicineCabinetId",
                        column: x => x.MedicineCabinetId,
                        principalTable: "MedicineCabinets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MedicineCabinetUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserMedications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PillSize = table.Column<int>(type: "int", nullable: true),
                    Validity = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    MedicineCabinetId = table.Column<int>(type: "int", nullable: false),
                    MedicationRepoId = table.Column<int>(type: "int", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMedications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMedications_MedicationRepos_MedicationRepoId",
                        column: x => x.MedicationRepoId,
                        principalTable: "MedicationRepos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserMedications_MedicineCabinets_MedicineCabinetId",
                        column: x => x.MedicineCabinetId,
                        principalTable: "MedicineCabinets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserMedications_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineCabinets_CreatorId",
                table: "MedicineCabinets",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineCabinetUsers_MedicineCabinetId",
                table: "MedicineCabinetUsers",
                column: "MedicineCabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMedications_CreatorId",
                table: "UserMedications",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMedications_MedicationRepoId",
                table: "UserMedications",
                column: "MedicationRepoId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMedications_MedicineCabinetId",
                table: "UserMedications",
                column: "MedicineCabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CabinetRequests");

            migrationBuilder.DropTable(
                name: "MedicineCabinetUsers");

            migrationBuilder.DropTable(
                name: "UserMedications");

            migrationBuilder.DropTable(
                name: "MedicationRepos");

            migrationBuilder.DropTable(
                name: "MedicineCabinets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
