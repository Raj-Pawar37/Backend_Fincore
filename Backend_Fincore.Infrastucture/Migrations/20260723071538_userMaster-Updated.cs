using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class userMasterUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is2FAEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TotpSecretKey",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OpexRequestId",
                table: "ExpenseClaims",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is2FAEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotpSecretKey",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "OpexRequestId",
                table: "ExpenseClaims",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
