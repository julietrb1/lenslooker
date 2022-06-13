using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddSonyLensFamilies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LensFamilies",
                columns: new[] { "Id", "CameraBrandId", "Name", "SensorFormat" },
                values: new object[,]
                {
                    { 6, 3, "DT", 1 },
                    { 7, 3, "DT G", 1 },
                    { 8, 3, "DT ZA", 1 },
                    { 9, 3, "E", 1 },
                    { 10, 3, "FE", 0 },
                    { 11, 3, "FE G", 0 },
                    { 12, 3, "FE ZA", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "LensFamilies",
                keyColumn: "Id",
                keyValue: 12);
        }
    }
}
