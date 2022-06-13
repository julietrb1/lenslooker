using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddBrandsAndFamiliesToContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Brand_BrandId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Brand_BrandId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_LensFamily_LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_LensFamily_Brand_CameraBrandId",
                table: "LensFamily");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LensFamily",
                table: "LensFamily");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brand",
                table: "Brand");

            migrationBuilder.RenameTable(
                name: "LensFamily",
                newName: "LensFamilies");

            migrationBuilder.RenameTable(
                name: "Brand",
                newName: "Brands");

            migrationBuilder.RenameIndex(
                name: "IX_LensFamily_CameraBrandId",
                table: "LensFamilies",
                newName: "IX_LensFamilies_CameraBrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LensFamilies",
                table: "LensFamilies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brands",
                table: "Brands",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cameras_Brands_BrandId",
                table: "Cameras",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Brands_BrandId",
                table: "Lenses",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_LensFamilies_LensFamilyId",
                table: "Lenses",
                column: "LensFamilyId",
                principalTable: "LensFamilies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LensFamilies_Brands_CameraBrandId",
                table: "LensFamilies",
                column: "CameraBrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cameras_Brands_BrandId",
                table: "Cameras");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Brands_BrandId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_LensFamilies_LensFamilyId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_LensFamilies_Brands_CameraBrandId",
                table: "LensFamilies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LensFamilies",
                table: "LensFamilies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brands",
                table: "Brands");

            migrationBuilder.RenameTable(
                name: "LensFamilies",
                newName: "LensFamily");

            migrationBuilder.RenameTable(
                name: "Brands",
                newName: "Brand");

            migrationBuilder.RenameIndex(
                name: "IX_LensFamilies_CameraBrandId",
                table: "LensFamily",
                newName: "IX_LensFamily_CameraBrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LensFamily",
                table: "LensFamily",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brand",
                table: "Brand",
                column: "Id");

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
                name: "FK_Lenses_LensFamily_LensFamilyId",
                table: "Lenses",
                column: "LensFamilyId",
                principalTable: "LensFamily",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LensFamily_Brand_CameraBrandId",
                table: "LensFamily",
                column: "CameraBrandId",
                principalTable: "Brand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
