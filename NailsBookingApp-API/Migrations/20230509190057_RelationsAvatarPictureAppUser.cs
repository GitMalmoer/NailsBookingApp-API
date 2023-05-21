using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NailsBookingApp_API.Migrations
{
    /// <inheritdoc />
    public partial class RelationsAvatarPictureAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvatarPictureId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                defaultValue: 8);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AvatarPictureId",
                table: "AspNetUsers",
                column: "AvatarPictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AvatarPictures_AvatarPictureId",
                table: "AspNetUsers",
                column: "AvatarPictureId",
                principalTable: "AvatarPictures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AvatarPictures_AvatarPictureId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AvatarPictureId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AvatarPictureId",
                table: "AspNetUsers");
        }
    }
}
