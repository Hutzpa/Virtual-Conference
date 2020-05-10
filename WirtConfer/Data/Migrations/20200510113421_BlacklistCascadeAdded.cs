using Microsoft.EntityFrameworkCore.Migrations;

namespace WirtConfer.Data.Migrations
{
    public partial class BlacklistCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_Events_EventId",
                table: "Blacklist");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_Events_EventId",
                table: "Blacklist",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_Events_EventId",
                table: "Blacklist");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_Events_EventId",
                table: "Blacklist",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
