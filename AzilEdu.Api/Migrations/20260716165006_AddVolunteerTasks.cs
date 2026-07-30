using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AzilEdu.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VolunteerTaskStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTaskStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerTaskTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTaskTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    VolunteerId = table.Column<int>(type: "INTEGER", nullable: true),
                    AnimalId = table.Column<int>(type: "INTEGER", nullable: true),
                    VolunteerTaskStatusId = table.Column<int>(type: "INTEGER", nullable: false),
                    VolunteerTaskTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerTasks_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VolunteerTasks_VolunteerTaskStatuses_VolunteerTaskStatusId",
                        column: x => x.VolunteerTaskStatusId,
                        principalTable: "VolunteerTaskStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerTasks_VolunteerTaskTypes_VolunteerTaskTypeId",
                        column: x => x.VolunteerTaskTypeId,
                        principalTable: "VolunteerTaskTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VolunteerTasks_Volunteers_VolunteerId",
                        column: x => x.VolunteerId,
                        principalTable: "Volunteers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "VolunteerTaskStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Otvoren" },
                    { 2, "Dodijeljen" },
                    { 3, "U tijeku" },
                    { 4, "Završeno" },
                    { 5, "Otkazano" }
                });

            migrationBuilder.InsertData(
                table: "VolunteerTaskTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Šetnja" },
                    { 2, "Hranjenje" },
                    { 3, "Čišćenje" },
                    { 4, "Socijalizacija" },
                    { 5, "Prijevoz" },
                    { 6, "Administracija" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTasks_AnimalId",
                table: "VolunteerTasks",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTasks_VolunteerId",
                table: "VolunteerTasks",
                column: "VolunteerId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTasks_VolunteerTaskStatusId",
                table: "VolunteerTasks",
                column: "VolunteerTaskStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerTasks_VolunteerTaskTypeId",
                table: "VolunteerTasks",
                column: "VolunteerTaskTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolunteerTasks");

            migrationBuilder.DropTable(
                name: "VolunteerTaskStatuses");

            migrationBuilder.DropTable(
                name: "VolunteerTaskTypes");
        }
    }
}
