using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class purchaseOrderitemUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuotationItemId",
                table: "PurchaseOrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PurchaseOrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GRNItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "GRNItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "IsActive",
                table: "GRNItem",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "GRNItem",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "GRNItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_QuotationItemId",
                table: "PurchaseOrderItems",
                column: "QuotationItemId",
                unique: true,
                filter: "[QuotationItemId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_QuotationItem_QuotationItemId",
                table: "PurchaseOrderItems",
                column: "QuotationItemId",
                principalTable: "QuotationItem",
                principalColumn: "QuotationItemId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_QuotationItem_QuotationItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_QuotationItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "QuotationItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "GRNItem");
        }
    }
}
