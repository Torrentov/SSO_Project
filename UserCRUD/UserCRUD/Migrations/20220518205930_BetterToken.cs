using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserCRUD.Migrations
{
    public partial class BetterToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_ApiToken_tokenaccess_token",
                table: "Tokens");

            migrationBuilder.DropTable(
                name: "ApiToken");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_tokenaccess_token",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "tokenaccess_token",
                table: "Tokens");

            migrationBuilder.AddColumn<string>(
                name: "token_type",
                table: "Tokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "token_type",
                table: "Tokens");

            migrationBuilder.AddColumn<string>(
                name: "tokenaccess_token",
                table: "Tokens",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiToken",
                columns: table => new
                {
                    access_token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    token_type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiToken", x => x.access_token);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_tokenaccess_token",
                table: "Tokens",
                column: "tokenaccess_token");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_ApiToken_tokenaccess_token",
                table: "Tokens",
                column: "tokenaccess_token",
                principalTable: "ApiToken",
                principalColumn: "access_token");
        }
    }
}
