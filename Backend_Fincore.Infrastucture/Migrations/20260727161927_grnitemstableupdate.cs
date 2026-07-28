using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class grnitemstableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRNItem_GRNs_GRNItemId",
                table: "GRNItem");

            migrationBuilder.AlterColumn<int>(
                name: "GRNItemId",
                table: "GRNItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_GRNItem_GRNId",
                table: "GRNItem",
                column: "GRNId");

            migrationBuilder.AddForeignKey(
                name: "FK_GRNItem_GRNs_GRNId",
                table: "GRNItem",
                column: "GRNId",
                principalTable: "GRNs",
                principalColumn: "GRNId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRNItem_GRNs_GRNId",
                table: "GRNItem");

            migrationBuilder.DropIndex(
                name: "IX_GRNItem_GRNId",
                table: "GRNItem");

            migrationBuilder.AlterColumn<int>(
                name: "GRNItemId",
                table: "GRNItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddForeignKey(
                name: "FK_GRNItem_GRNs_GRNItemId",
                table: "GRNItem",
                column: "GRNItemId",
                principalTable: "GRNs",
                principalColumn: "GRNId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
