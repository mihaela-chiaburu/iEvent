using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserProfileWithIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AdminUsers_AdminId",
                table: "AuditLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AdminUsers_AdminId",
                table: "AuditLogs",
                column: "AdminId",
                principalTable: "AdminUsers",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AdminUsers_AdminId",
                table: "AuditLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AdminUsers_AdminId",
                table: "AuditLogs",
                column: "AdminId",
                principalTable: "AdminUsers",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
