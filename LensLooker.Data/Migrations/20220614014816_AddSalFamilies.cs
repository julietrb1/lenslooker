using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddSalFamilies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LensFamilies",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[] { 14, 3, "SAL", 0 });

            migrationBuilder.InsertData(
                table: "LensFamilies",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[] { 15, 3, "SAL G", 0 });

            migrationBuilder.InsertData(
                table: "LensFamilies",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[] { 16, 3, "SAL ZA", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
