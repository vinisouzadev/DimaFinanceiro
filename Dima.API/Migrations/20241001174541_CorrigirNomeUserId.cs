using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dima.API.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirNomeUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UderId",
                table: "Transaction",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Transaction",
                newName: "UderId");
        }
    }
}
