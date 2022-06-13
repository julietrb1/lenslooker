using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddBrandAndLensFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_LensMount_LensMountId",
                table: "Lenses");

            migrationBuilder.DropTable(
                name: "LensMount");

            migrationBuilder.RenameColumn(
                name: "LensMountId",
                table: "Lenses",
                newName: "LensFamilyId");

            migrationBuilder.RenameIndex(
                name: "IX_Lenses_LensMountId",
                table: "Lenses",
                newName: "IX_Lenses_LensFamilyId");

            migrationBuilder.CreateTable(
                name: "LensFamily",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraBrandId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SensorFormat = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LensFamily", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LensFamily_Brand_CameraBrandId",
                        column: x => x.CameraBrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brand",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Canon" });

            migrationBuilder.InsertData(
                table: "Brand",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Nikon" });

            migrationBuilder.InsertData(
                table: "Brand",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Sony" });

            migrationBuilder.InsertData(
                table: "LensFamily",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[,]
                {
                    { 1, 1, "EF", 0 },
                    { 2, 1, "EF L", 0 },
                    { 3, 1, "EF-S", 1 },
                    { 4, 1, "RF", 0 },
                    { 5, 1, "RF L", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LensFamily_CameraBrandId",
                table: "LensFamily",
                column: "CameraBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_LensFamily_LensFamilyId",
                table: "Lenses",
                column: "LensFamilyId",
                principalTable: "LensFamily",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_LensFamily_LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropTable(
                name: "LensFamily");

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brand",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "LensFamilyId",
                table: "Lenses",
                newName: "LensMountId");

            migrationBuilder.RenameIndex(
                name: "IX_Lenses_LensFamilyId",
                table: "Lenses",
                newName: "IX_Lenses_LensMountId");

            migrationBuilder.CreateTable(
                name: "LensMount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CameraBrandId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SensorFormat = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LensMount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LensMount_Brand_CameraBrandId",
                        column: x => x.CameraBrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LensMount_CameraBrandId",
                table: "LensMount",
                column: "CameraBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_LensMount_LensMountId",
                table: "Lenses",
                column: "LensMountId",
                principalTable: "LensMount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
