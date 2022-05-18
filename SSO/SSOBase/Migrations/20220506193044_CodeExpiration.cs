using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSOBase.Migrations
{
    public partial class CodeExpiration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeSetTime",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeSetTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }
    }
}
