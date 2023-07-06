using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NailsBookingApp_API.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalSubjectIdAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalSubjectId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalSubjectId",
                table: "AspNetUsers");
        }
    }
}
