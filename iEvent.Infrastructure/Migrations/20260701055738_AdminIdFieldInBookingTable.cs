using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdminIdFieldInBookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CollectedAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CollectedById",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AdminUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CollectedById",
                table: "Bookings",
                column: "CollectedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_AdminUsers_CollectedById",
                table: "Bookings",
                column: "CollectedById",
                principalTable: "AdminUsers",
                principalColumn: "AdminId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_AdminUsers_CollectedById",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CollectedById",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CollectedAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CollectedById",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AdminUsers");
        }
    }
}
