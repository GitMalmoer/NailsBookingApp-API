using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NailsBookingApp_API.Migrations
{
    /// <inheritdoc />
    public partial class AddingPasswordResetProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PassResetExpirationDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassResetToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassResetExpirationDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PassResetToken",
                table: "AspNetUsers");
        }
    }
}
