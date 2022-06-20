using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class AddIndexForCommonPhotoFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoId_LensId_IsExifFetched",
                table: "Photos",
                columns: new[] { "PhotoId", "LensId", "IsExifFetched" })
                .Annotation("SqlServer:Include", new[] { "CameraId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Photos_PhotoId_LensId_IsExifFetched",
                table: "Photos");
        }
    }
}
