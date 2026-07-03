using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCloudinaryPublicIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "VenueImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloudinaryPublicId",
                table: "EventImages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "VenueImages");

            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CloudinaryPublicId",
                table: "EventImages");
        }
    }
}
