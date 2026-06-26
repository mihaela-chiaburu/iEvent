using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TimeSlotsAndTaxForBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdminFee",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "BookingTimeSlotId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingTimeSlotId",
                table: "Bookings",
                column: "BookingTimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_EventTimeSlots_BookingTimeSlotId",
                table: "Bookings",
                column: "BookingTimeSlotId",
                principalTable: "EventTimeSlots",
                principalColumn: "TimeSlotId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_EventTimeSlots_BookingTimeSlotId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingTimeSlotId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "AdminFee",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingTimeSlotId",
                table: "Bookings");
        }
    }
}
