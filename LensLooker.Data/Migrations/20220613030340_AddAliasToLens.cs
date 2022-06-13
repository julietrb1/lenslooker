using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddAliasToLens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AliasOfName",
                table: "Lenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lenses_AliasOfName",
                table: "Lenses",
                column: "AliasOfName");

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Lenses_AliasOfName",
                table: "Lenses",
                column: "AliasOfName",
                principalTable: "Lenses",
                principalColumn: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropColumn(
                name: "AliasOfName",
                table: "Lenses");
        }
    }
}
