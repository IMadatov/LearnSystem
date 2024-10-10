using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnSystem.Migrations
{
    /// <inheritdoc />
    public partial class addedStatususer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StatusUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsOnTelegramBotActive = table.Column<bool>(type: "bit", nullable: false),
                    hasPhotoProfile = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StatusUserId",
                table: "AspNetUsers",
                column: "StatusUserId",
                unique: true,
                filter: "[StatusUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_StatusUsers_StatusUserId",
                table: "AspNetUsers",
                column: "StatusUserId",
                principalTable: "StatusUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_StatusUsers_StatusUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "StatusUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StatusUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StatusUserId",
                table: "AspNetUsers");
        }
    }
}
