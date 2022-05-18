using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserCRUD.Migrations
{
    public partial class UserToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiToken",
                columns: table => new
                {
                    token_type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    access_token = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiToken", x => x.token_type);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    token_type = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.email);
                    table.ForeignKey(
                        name: "FK_Tokens_ApiToken_token_type",
                        column: x => x.token_type,
                        principalTable: "ApiToken",
                        principalColumn: "token_type");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_token_type",
                table: "Tokens",
                column: "token_type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "ApiToken");
        }
    }
}
