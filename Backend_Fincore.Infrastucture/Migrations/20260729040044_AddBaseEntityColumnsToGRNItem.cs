using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_Fincore.Migrations
{
    public partial class AddBaseEntityColumnsToGRNItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "IsActive",
                table: "GRNItem",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "GRNItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GRNItem",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "GRNItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "GRNItem",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "GRNItem");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "GRNItem");
        }
    }
}