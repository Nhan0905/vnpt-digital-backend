using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vnpt.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalLogs_Orders_OrderId",
                table: "ApprovalLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalLogs_Users_ProcessedById",
                table: "ApprovalLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalLogs_OrderId",
                table: "ApprovalLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalLogs_ProcessedById",
                table: "ApprovalLogs");

            migrationBuilder.DropColumn(
                name: "ProcessedById",
                table: "ApprovalLogs");

            migrationBuilder.RenameColumn(
                name: "ProcessedBy_UserId",
                table: "ApprovalLogs",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ApprovalLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ApprovalLogs",
                newName: "ProcessedBy_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ApprovalLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcessedById",
                table: "ApprovalLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLogs_OrderId",
                table: "ApprovalLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLogs_ProcessedById",
                table: "ApprovalLogs",
                column: "ProcessedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLogs_Orders_OrderId",
                table: "ApprovalLogs",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalLogs_Users_ProcessedById",
                table: "ApprovalLogs",
                column: "ProcessedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
