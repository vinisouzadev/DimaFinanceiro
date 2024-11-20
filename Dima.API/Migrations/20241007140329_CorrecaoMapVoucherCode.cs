using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dima.API.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoMapVoucherCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VourcherCode",
                table: "Voucher",
                type: "VARCHAR",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "CHAR",
                oldMaxLength: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VourcherCode",
                table: "Voucher",
                type: "CHAR",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR",
                oldMaxLength: 8);
        }
    }
}
