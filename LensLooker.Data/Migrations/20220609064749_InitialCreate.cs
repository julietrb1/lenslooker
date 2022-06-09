using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LensLooker.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cameras",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cameras", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Lenses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lenses", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Farm = table.Column<int>(type: "int", nullable: false),
                    Server = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsExifFetched = table.Column<bool>(type: "bit", nullable: false),
                    IsSkipped = table.Column<bool>(type: "bit", nullable: false),
                    FNumber = table.Column<double>(type: "float", nullable: true),
                    ExposureTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iso = table.Column<int>(type: "int", nullable: true),
                    FocalLengthInMm = table.Column<int>(type: "int", nullable: true),
                    CameraName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LensName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateTimeShot = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_Photos_Cameras_CameraName",
                        column: x => x.CameraName,
                        principalTable: "Cameras",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Photos_Lenses_LensName",
                        column: x => x.LensName,
                        principalTable: "Lenses",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_CameraName",
                table: "Photos",
                column: "CameraName");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_LensName",
                table: "Photos",
                column: "LensName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Cameras");

            migrationBuilder.DropTable(
                name: "Lenses");
        }
    }
}
