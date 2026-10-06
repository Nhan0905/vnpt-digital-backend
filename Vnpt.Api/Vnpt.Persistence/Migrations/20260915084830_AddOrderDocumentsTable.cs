using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vnpt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderDocumentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrderDocuments");

            migrationBuilder.RenameColumn(
                name: "DocumentType",
                table: "OrderDocuments",
                newName: "FileType");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderDocuments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "OrderDocuments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "OrderDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderDocuments");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "OrderDocuments");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OrderDocuments");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "OrderDocuments",
                newName: "DocumentType");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrderDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
