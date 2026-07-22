using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class GRNItemsCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GRNItem",
                columns: table => new
                {
                    GRNItemId = table.Column<int>(type: "int", nullable: false),
                    GRNId = table.Column<int>(type: "int", nullable: false),
                    POItemId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRNItem", x => x.GRNItemId);
                    table.ForeignKey(
                        name: "FK_GRNItem_GRNs_GRNItemId",
                        column: x => x.GRNItemId,
                        principalTable: "GRNs",
                        principalColumn: "GRNId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRNItem_PurchaseOrderItems_POItemId",
                        column: x => x.POItemId,
                        principalTable: "PurchaseOrderItems",
                        principalColumn: "POItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GRNItem_POItemId",
                table: "GRNItem",
                column: "POItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRNItem");
        }
    }
}
