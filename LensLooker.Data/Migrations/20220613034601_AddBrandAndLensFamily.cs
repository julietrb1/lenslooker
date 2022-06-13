using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddBrandAndLensFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AliasOfName",
                table: "Lenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Lenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LensFamilyId",
                table: "Lenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Cameras",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

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
                name: "IX_Lenses_AliasOfName",
                table: "Lenses",
                column: "AliasOfName");

            migrationBuilder.CreateIndex(
                name: "IX_Lenses_BrandId",
                table: "Lenses",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Lenses_LensFamilyId",
                table: "Lenses",
                column: "LensFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_BrandId",
                table: "Cameras",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_LensFamily_CameraBrandId",
                table: "LensFamily",
                column: "CameraBrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Brand_BrandId",
                table: "Cameras",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Brand_BrandId",
                table: "Lenses",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Lenses_AliasOfName",
                table: "Lenses",
                column: "AliasOfName",
                principalTable: "Lenses",
                principalColumn: "Name");

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
                name: "FK_Cameras_Brand_BrandId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Brand_BrandId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_LensFamily_LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropTable(
                name: "LensFamily");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_BrandId",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_BrandId",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "AliasOfName",
                table: "Lenses");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Lenses");

            migrationBuilder.DropColumn(
                name: "LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Cameras");
        }
    }
}
