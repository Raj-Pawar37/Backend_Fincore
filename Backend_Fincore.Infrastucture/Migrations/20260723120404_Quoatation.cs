using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class Quoatation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_RFQVendors_RFQVendorId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RFQVendorId",
                table: "Quotations");

            migrationBuilder.AddColumn<int>(
                name: "RFQId",
                table: "Quotations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RFQVendorId",
                table: "Quotations",
                column: "RFQVendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_RFQVendors_RFQVendorId",
                table: "Quotations",
                column: "RFQVendorId",
                principalTable: "RFQVendors",
                principalColumn: "RFQVendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_RFQVendors_RFQVendorId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RFQVendorId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "RFQId",
                table: "Quotations");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RFQVendorId",
                table: "Quotations",
                column: "RFQVendorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_RFQVendors_RFQVendorId",
                table: "Quotations",
                column: "RFQVendorId",
                principalTable: "RFQVendors",
                principalColumn: "RFQVendorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
