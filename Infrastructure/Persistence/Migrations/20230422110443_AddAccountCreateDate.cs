using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NailsBookingApp_API.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountCreateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AccountCreateDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountCreateDate",
                table: "AspNetUsers");
        }
    }
}
