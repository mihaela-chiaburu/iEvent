using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iEvent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributeForEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Events");
        }
    }
}
