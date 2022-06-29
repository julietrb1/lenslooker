using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class CreateUniqueIdOnPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_CameraId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoId_LensId_IsExifFetched",
                table: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Photos",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CameraId_LensId_IsExifFetched",
                table: "Photos",
                columns: new[] { "CameraId", "LensId", "IsExifFetched" });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoId",
                table: "Photos",
                column: "PhotoId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_CameraId_LensId_IsExifFetched",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Photos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "PhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CameraId",
                table: "Photos",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoId_LensId_IsExifFetched",
                table: "Photos",
                columns: new[] { "PhotoId", "LensId", "IsExifFetched" })
                .Annotation("SqlServer:Include", new[] { "CameraId" });
        }
    }
}
