using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusServicesPortal.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEventRegistrationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "EventRegistrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "EventRegistrations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventRegistrations");
        }
    }
}
