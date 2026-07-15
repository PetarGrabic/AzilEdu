using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AzilEdu.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimalStatusRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAdopted",
                table: "Animals",
                newName: "AnimalStatusId");

            migrationBuilder.CreateTable(
                name: "AnimalStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AnimalStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dostupna za udomljenje" },
                    { 2, "Rezervirana" },
                    { 3, "Udomljena" },
                    { 4, "Na liječenju" }
                });

            // AnimalStatusId still holds the old IsAdopted boolean (0/1) at this point;
            // map it onto valid AnimalStatuses ids before the FK constraint is added.
            migrationBuilder.Sql(
                "UPDATE \"Animals\" SET \"AnimalStatusId\" = CASE WHEN \"AnimalStatusId\" = 1 THEN 3 ELSE 1 END;");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_AnimalStatusId",
                table: "Animals",
                column: "AnimalStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AnimalStatuses_AnimalStatusId",
                table: "Animals",
                column: "AnimalStatusId",
                principalTable: "AnimalStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AnimalStatuses_AnimalStatusId",
                table: "Animals");

            migrationBuilder.DropTable(
                name: "AnimalStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Animals_AnimalStatusId",
                table: "Animals");

            // Map AnimalStatusId back onto the boolean domain before renaming back to IsAdopted.
            migrationBuilder.Sql(
                "UPDATE \"Animals\" SET \"AnimalStatusId\" = CASE WHEN \"AnimalStatusId\" = 3 THEN 1 ELSE 0 END;");

            migrationBuilder.RenameColumn(
                name: "AnimalStatusId",
                table: "Animals",
                newName: "IsAdopted");
        }
    }
}
