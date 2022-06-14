using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class ConvertPrimaryKeyOnCameraAndLens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Cameras_CameraName",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Lenses_LensName",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_CameraName",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_LensName",
                table: "Photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lenses",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_AliasOfName",
                table: "Lenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cameras",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "CameraName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "LensName",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "AliasOfName",
                table: "Lenses");

            migrationBuilder.AddColumn<int>(
                name: "CameraId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LensId",
                table: "Photos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AliasOfId",
                table: "Lenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lenses",
                table: "Lenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cameras",
                table: "Cameras",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CameraId",
                table: "Photos",
                column: "CameraId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_LensId",
                table: "Photos",
                column: "LensId");

            migrationBuilder.CreateIndex(
                name: "IX_Lenses_AliasOfId",
                table: "Lenses",
                column: "AliasOfId");

            migrationBuilder.CreateIndex(
                name: "IX_Cameras_Name",
                table: "Cameras",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lenses_Lenses_AliasOfId",
                table: "Lenses",
                column: "AliasOfId",
                principalTable: "Lenses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Cameras_CameraId",
                table: "Photos",
                column: "CameraId",
                principalTable: "Cameras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Lenses_LensId",
                table: "Photos",
                column: "LensId",
                principalTable: "Lenses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lenses_Lenses_AliasOfId",
                table: "Lenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Cameras_CameraId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Lenses_LensId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_CameraId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_LensId",
                table: "Photos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lenses",
                table: "Lenses");

            migrationBuilder.DropIndex(
                name: "IX_Lenses_AliasOfId",
                table: "Lenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cameras",
                table: "Cameras");

            migrationBuilder.DropIndex(
                name: "IX_Cameras_Name",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "CameraId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "LensId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "AliasOfId",
                table: "Lenses");

            migrationBuilder.AddColumn<string>(
                name: "CameraName",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LensName",
                table: "Photos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AliasOfName",
                table: "Lenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lenses",
                table: "Lenses",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cameras",
                table: "Cameras",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CameraName",
                table: "Photos",
                column: "CameraName");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_LensName",
                table: "Photos",
                column: "LensName");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Cameras_CameraName",
                table: "Photos",
                column: "CameraName",
                principalTable: "Cameras",
                principalColumn: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Lenses_LensName",
                table: "Photos",
                column: "LensName",
                principalTable: "Lenses",
                principalColumn: "Name");
        }
    }
}
