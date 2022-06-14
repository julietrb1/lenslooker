using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class UniquelyIdentifyLensByFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lenses_Name_LensFamilyId",
                table: "Lenses",
                columns: new[] { "Name", "LensFamilyId" },
                unique: true,
                filter: "[LensFamilyId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lenses_Name_LensFamilyId",
                table: "Lenses");
        }
    }
}
