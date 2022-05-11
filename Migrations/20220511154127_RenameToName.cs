using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kona.Migrations
{
    public partial class RenameToName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Subscribes",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Subscribes",
                newName: "name");
        }
    }
}
