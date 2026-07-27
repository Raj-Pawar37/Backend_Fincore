using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class QuotationtableRFQid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "PurchaseOrderItems");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionGroupId",
                table: "JournalEntries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionGroupId",
                table: "JournalEntries");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "PurchaseOrderItems",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
