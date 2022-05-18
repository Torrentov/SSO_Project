using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserCRUD.Migrations
{
    public partial class ApiKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_ApiToken_token_type",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiToken",
                table: "ApiToken");

            migrationBuilder.RenameColumn(
                name: "token_type",
                table: "Tokens",
                newName: "tokenaccess_token");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_token_type",
                table: "Tokens",
                newName: "IX_Tokens_tokenaccess_token");

            migrationBuilder.AlterColumn<string>(
                name: "access_token",
                table: "ApiToken",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "token_type",
                table: "ApiToken",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiToken",
                table: "ApiToken",
                column: "access_token");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_ApiToken_tokenaccess_token",
                table: "Tokens",
                column: "tokenaccess_token",
                principalTable: "ApiToken",
                principalColumn: "access_token");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_ApiToken_tokenaccess_token",
                table: "Tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiToken",
                table: "ApiToken");

            migrationBuilder.RenameColumn(
                name: "tokenaccess_token",
                table: "Tokens",
                newName: "token_type");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_tokenaccess_token",
                table: "Tokens",
                newName: "IX_Tokens_token_type");

            migrationBuilder.AlterColumn<string>(
                name: "token_type",
                table: "ApiToken",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "access_token",
                table: "ApiToken",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiToken",
                table: "ApiToken",
                column: "token_type");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_ApiToken_token_type",
                table: "Tokens",
                column: "token_type",
                principalTable: "ApiToken",
                principalColumn: "token_type");
        }
    }
}
