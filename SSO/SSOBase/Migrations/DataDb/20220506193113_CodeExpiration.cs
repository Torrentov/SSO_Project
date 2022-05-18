using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SSOBase.Migrations.DataDb
{
    public partial class CodeExpiration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeExpirationTime",
                table: "Datas",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExpirationTime",
                table: "Datas");
        }
    }
}
