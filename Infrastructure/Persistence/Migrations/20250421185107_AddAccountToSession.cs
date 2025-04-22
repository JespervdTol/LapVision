using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "Session",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Session_AccountID",
                table: "Session",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Account_AccountID",
                table: "Session",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_Account_AccountID",
                table: "Session");

            migrationBuilder.DropIndex(
                name: "IX_Session_AccountID",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "Session");
        }
    }
}
