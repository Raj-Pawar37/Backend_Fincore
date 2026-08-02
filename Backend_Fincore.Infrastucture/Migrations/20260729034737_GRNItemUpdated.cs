using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    /// <inheritdoc />
    public partial class GRNItemUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
             * The original GRNItem table has multiple structural problems:
             *
             * 1. GRNItemId is not an Identity column.
             * 2. GRN foreign key incorrectly uses GRNItemId.
             * 3. POItemId has a unique index.
             *
             * Because SQL Server cannot directly change Identity,
             * recreate the table.
             */

            //migrationBuilder.DropTable(
            //    name: "GRNItem");

            migrationBuilder.CreateTable(
                name: "GRNItem",
                columns: table => new
                {
                    GRNItemId = table.Column<int>(
                            type: "int",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    GRNId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    POItemId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Remarks = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true),

                    Qty = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_GRNItem",
                        columns: x => x.GRNItemId);

                    table.ForeignKey(
                        name: "FK_GRNItem_GRNs_GRNId",
                        column: x => x.GRNId,
                        principalTable: "GRNs",
                        principalColumn: "GRNId",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_GRNItem_PurchaseOrderItems_POItemId",
                        column: x => x.POItemId,
                        principalTable: "PurchaseOrderItems",
                        principalColumn: "POItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GRNItem_GRNId",
                table: "GRNItem",
                column: "GRNId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNItem_POItemId",
                table: "GRNItem",
                column: "POItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRNItem");

            // Recreates the previous incorrect structure during rollback.
            migrationBuilder.CreateTable(
                name: "GRNItem",
                columns: table => new
                {
                    GRNItemId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    GRNId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    POItemId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Remarks = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: true),

                    Qty = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        name: "PK_GRNItem",
                        columns: x => x.GRNItemId);

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
    }
}