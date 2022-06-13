using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddEfMLensFamily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LensFamilies",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[] { 13, 1, "EF-M", 1 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 13);
        }
    }
}
