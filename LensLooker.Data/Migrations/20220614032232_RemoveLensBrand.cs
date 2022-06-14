using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class RemoveLensBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Brands_BrandId",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_BrandId",
                table: "Lenses");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Lenses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Lenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lenses_BrandId",
                table: "Lenses",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Brands_BrandId",
                table: "Lenses",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");
        }
    }
}
