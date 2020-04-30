using Microsoft.EntityFrameworkCore.Migrations;

namespace WirtConfer.Data.Migrations
{
    public partial class DeletedInviteStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Invites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Invites",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
