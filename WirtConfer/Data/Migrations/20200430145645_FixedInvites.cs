using Microsoft.EntityFrameworkCore.Migrations;

namespace WirtConfer.Data.Migrations
{
    public partial class FixedInvites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Invites",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invites");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Invites");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
