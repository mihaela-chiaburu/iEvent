using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantImageUrlFromEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventImages_EventId",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "IsBanner",
                table: "EventImages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_EventId_IsBanner",
                table: "EventImages",
                columns: new[] { "EventId", "IsBanner" },
                unique: true,
                filter: "[IsBanner] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventImages_EventId_IsBanner",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "IsBanner",
                table: "EventImages");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventImages_EventId",
                table: "EventImages",
                column: "EventId");
        }
    }
}
